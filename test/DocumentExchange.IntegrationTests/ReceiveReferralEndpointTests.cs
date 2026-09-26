using System.Net;
using System.Net.Http.Json;
using DocumentExchange.Api.Contracts;
using DocumentExchange.Api.Data;
using DocumentExchange.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentExchange.IntegrationTests;

[Collection(ApiCollection.Name)]
public class ReceiveReferralEndpointTests(WebApplicationFactory<Program> factory) : ApiCollectionTestBase(factory)
{
    private const string Bsn = "999990019";
    private static readonly DateOnly DateOfBirth = new(1942, 3, 14);

    [Fact]
    public async Task ValidReferral_ReturnsCreatedWithStoredReferral()
    {
        var client = Factory.CreateClient();
        var request = CreateRequest([new Allergy("Penicillin", "Skin rash")]);

        var response = await client.PostAsJsonAsync("/api/referrals", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var referral = await response.Content.ReadFromJsonAsync<Referral>();
        Assert.NotNull(referral);
        Assert.NotEqual(Guid.Empty, referral.Id);
        Assert.Equal(request.Reason, referral.Reason);
        Assert.Equal(Bsn, referral.Patient.Bsn);
        Assert.Equal([new Allergy("Penicillin", "Skin rash")], referral.Patient.Allergies);
    }

    [Fact]
    public async Task ValidReferral_StoresReferralAndPatient()
    {
        var client = Factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/referrals", CreateRequest([new Allergy("Penicillin", "Skin rash")]));

        var referral = await response.Content.ReadFromJsonAsync<Referral>();
        Assert.True(Database.Referrals.ContainsKey(referral!.Id));
        Assert.True(Database.Patients.ContainsKey(Bsn));
    }

    [Fact]
    public async Task KnownPatient_IsMergedInsteadOfReplaced()
    {
        var patients = Factory.Services.GetRequiredService<IPatientRepository>();
        await patients.AddOrMerge(new Patient(Bsn, "Jan Jansen", DateOfBirth, [new Allergy("Latex", "Itching")]));
        var client = Factory.CreateClient();

        await client.PostAsJsonAsync("/api/referrals", CreateRequest([new Allergy("Penicillin", "Skin rash")]));

        var patient = await patients.GetByBsnAsync(Bsn);
        Assert.NotNull(patient);
        Assert.Equal(2, patient.Allergies.Count);
        Assert.Contains(new Allergy("Latex", "Itching"), patient.Allergies);
        Assert.Contains(new Allergy("Penicillin", "Skin rash"), patient.Allergies);
    }

    [Fact]
    public async Task SameReferralTwice_StoresTwoReferrals()
    {
        var client = Factory.CreateClient();
        var request = CreateRequest([]);

        await client.PostAsJsonAsync("/api/referrals", request);
        await client.PostAsJsonAsync("/api/referrals", request);

        Assert.Equal(2, Database.Referrals.Count);
        Assert.Single(Database.Patients);
    }

    private static ReferralRequest CreateRequest(IReadOnlyList<Allergy> allergies) =>
        new(new Patient(Bsn, "Jan Jansen", DateOfBirth, allergies), "Discharged after hip surgery.");
}
