using System.Net;
using System.Net.Http.Json;
using System.Text;
using DocumentExchange.Api.Contracts;
using DocumentExchange.Api.Data;
using DocumentExchange.Api.Models;
using Microsoft.AspNetCore.Http;
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
        var request = CreateRequest([new ReferralAllergyRequest("Penicillin", "Skin rash")]);

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

        var response = await client.PostAsJsonAsync("/api/referrals", CreateRequest([new ReferralAllergyRequest("Penicillin", "Skin rash")]));

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

        await client.PostAsJsonAsync("/api/referrals", CreateRequest([new ReferralAllergyRequest("Penicillin", "Skin rash")]));

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

    [Theory]
    [InlineData("""{ "reason": "Discharged" }""", "Patient")]
    [InlineData("""{ "patient": { "bsn": "999990019", "name": "Jan Jansen", "dateOfBirth": "1942-03-14", "allergies": [] } }""", "Reason")]
    [InlineData("""{ "patient": { "name": "Jan Jansen", "dateOfBirth": "1942-03-14", "allergies": [] }, "reason": "Discharged" }""", "Patient.Bsn")]
    [InlineData("""{ "patient": { "bsn": "12345678", "name": "Jan Jansen", "dateOfBirth": "1942-03-14", "allergies": [] }, "reason": "Discharged" }""", "Patient.Bsn")]
    [InlineData("""{ "patient": { "bsn": "12345678a", "name": "Jan Jansen", "dateOfBirth": "1942-03-14", "allergies": [] }, "reason": "Discharged" }""", "Patient.Bsn")]
    [InlineData("""{ "patient": { "bsn": "999990019", "name": "", "dateOfBirth": "1942-03-14", "allergies": [] }, "reason": "Discharged" }""", "Patient.Name")]
    [InlineData("""{ "patient": { "bsn": "999990019", "name": "Jan Jansen", "dateOfBirth": "1942-03-14", "allergies": null }, "reason": "Discharged" }""", "Patient.Allergies")]
    [InlineData("""{ "patient": { "bsn": "999990019", "name": "Jan Jansen", "dateOfBirth": "1942-03-14", "allergies": [ { "substance": "", "reaction": "Itching" } ] }, "reason": "Discharged" }""", "Patient.Allergies[0].Substance")]
    [InlineData("""{ "patient": { "bsn": "999990019", "name": "Jan Jansen", "dateOfBirth": "1942-03-14", "allergies": [ { "substance": "Latex" } ] }, "reason": "Discharged" }""", "Patient.Allergies[0].Reaction")]
    public async Task InvalidReferral_ReturnsValidationProblemAndStoresNothing(string json, string expectedErrorKey)
    {
        var client = Factory.CreateClient();

        var response = await client.PostAsync("/api/referrals", new StringContent(json, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains(expectedErrorKey, problem.Errors.Keys);
        Assert.Empty(Database.Referrals);
        Assert.Empty(Database.Patients);
    }

    [Fact]
    public async Task MalformedJson_ReturnsBadRequest()
    {
        var client = Factory.CreateClient();

        var response = await client.PostAsync("/api/referrals", new StringContent("{ not json", Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(Database.Referrals);
    }

    private static ReferralRequest CreateRequest(IReadOnlyList<ReferralAllergyRequest> allergies) =>
        new(new ReferralPatientRequest(Bsn, "Jan Jansen", DateOfBirth, allergies), "Discharged after hip surgery.");
}
