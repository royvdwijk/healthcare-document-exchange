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
        var client = CreateIdentifiedClient();
        var request = CreateRequest([new ReferralAllergyRequest("Penicillin", "Skin rash")]);

        var response = await client.PostAsJsonAsync("/api/referrals", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var referral = await response.Content.ReadFromJsonAsync<Referral>();
        Assert.NotNull(referral);
        Assert.NotEqual(Guid.Empty, referral.Id);
        Assert.Equal(Identity, referral.Owner);
        Assert.Equal(request.Reason, referral.Reason);
        Assert.Equal(Bsn, referral.Patient.Bsn);
        Assert.Equal([new Allergy("Penicillin", "Skin rash")], referral.Patient.Allergies);
    }

    [Fact]
    public async Task ValidReferral_StoresReferralAndPatient()
    {
        var client = CreateIdentifiedClient();

        var response = await client.PostAsJsonAsync("/api/referrals", CreateRequest([new ReferralAllergyRequest("Penicillin", "Skin rash")]));

        var referral = await response.Content.ReadFromJsonAsync<Referral>();
        Assert.True(Database.Referrals.ContainsKey(referral!.Id));
        Assert.True(Database.Patients.ContainsKey(Bsn));
    }

    [Fact]
    public async Task ValidReferral_StoresIdentityAsOwner()
    {
        var client = CreateIdentifiedClient();

        var response = await client.PostAsJsonAsync("/api/referrals", CreateRequest([]));

        var referral = await response.Content.ReadFromJsonAsync<Referral>();
        Assert.True(Database.Referrals.TryGetValue(referral!.Id, out var stored));
        Assert.Equal(Identity, stored.Owner);
    }

    [Fact]
    public async Task KnownPatient_IsMergedInsteadOfReplaced()
    {
        var patients = Factory.Services.GetRequiredService<IPatientRepository>();
        await patients.AddOrMerge(new Patient(Bsn, "Jan Jansen", DateOfBirth, [new Allergy("Latex", "Itching")], []));
        var client = CreateIdentifiedClient();

        await client.PostAsJsonAsync("/api/referrals", CreateRequest([new ReferralAllergyRequest("Penicillin", "Skin rash")]));

        var patient = await patients.GetByBsnAsync(Bsn);
        Assert.NotNull(patient);
        Assert.Equal(2, patient.Allergies.Count);
        Assert.Contains(new Allergy("Latex", "Itching"), patient.Allergies);
        Assert.Contains(new Allergy("Penicillin", "Skin rash"), patient.Allergies);
    }

    [Fact]
    public async Task KnownPatient_KeepsMedications()
    {
        var patients = Factory.Services.GetRequiredService<IPatientRepository>();
        await patients.AddOrMerge(new Patient(Bsn, "Jan Jansen", DateOfBirth, [], [new Medication("Metoprolol", "50 mg", "Once a day")]));
        var client = CreateIdentifiedClient();

        await client.PostAsJsonAsync("/api/referrals", CreateRequest([]));

        var patient = await patients.GetByBsnAsync(Bsn);
        Assert.NotNull(patient);
        Assert.Equal([new Medication("Metoprolol", "50 mg", "Once a day")], patient.Medications);
    }

    [Fact]
    public async Task WithoutDateOfBirth_StoresPatientWithoutDateOfBirth()
    {
        var client = CreateIdentifiedClient();
        const string json = """{ "patient": { "bsn": "999990019", "name": "Jan Jansen", "allergies": [] }, "reason": "Discharged" }""";

        var response = await client.PostAsync("/api/referrals", new StringContent(json, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(Database.Patients.TryGetValue(Bsn, out var patient));
        Assert.Null(patient.DateOfBirth);
    }

    [Fact]
    public async Task SameReferralTwice_StoresTwoReferrals()
    {
        var client = CreateIdentifiedClient();
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
    [InlineData("""{ "patient": { "bsn": "123456780", "name": "Jan Jansen", "dateOfBirth": "1942-03-14", "allergies": [] }, "reason": "Discharged" }""", "Patient.Bsn")]
    [InlineData("""{ "patient": { "bsn": "９９９９９００１９", "name": "Jan Jansen", "dateOfBirth": "1942-03-14", "allergies": [] }, "reason": "Discharged" }""", "Patient.Bsn")]
    [InlineData("""{ "patient": { "bsn": "999990019", "name": "", "dateOfBirth": "1942-03-14", "allergies": [] }, "reason": "Discharged" }""", "Patient.Name")]
    [InlineData("""{ "patient": { "bsn": "999990019", "name": "Jan Jansen", "dateOfBirth": "1942-03-14", "allergies": null }, "reason": "Discharged" }""", "Patient.Allergies")]
    [InlineData("""{ "patient": { "bsn": "999990019", "name": "Jan Jansen", "dateOfBirth": "1942-03-14", "allergies": [ { "substance": "", "reaction": "Itching" } ] }, "reason": "Discharged" }""", "Patient.Allergies[0].Substance")]
    [InlineData("""{ "patient": { "bsn": "999990019", "name": "Jan Jansen", "dateOfBirth": "1942-03-14", "allergies": [ { "substance": "Latex" } ] }, "reason": "Discharged" }""", "Patient.Allergies[0].Reaction")]
    public async Task InvalidReferral_ReturnsValidationProblemAndStoresNothing(string json, string expectedErrorKey)
    {
        var client = CreateIdentifiedClient();

        var response = await client.PostAsync("/api/referrals", new StringContent(json, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains(expectedErrorKey, problem.Errors.Keys);
        Assert.Empty(Database.Referrals);
        Assert.Empty(Database.Patients);
    }

    public static TheoryData<ReferralRequest, string> InvalidRequests => new()
    {
        { CreateRequest([], name: new string('a', 201)), "Patient.Name" },
        { CreateRequest([], dateOfBirth: DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1)), "Patient.DateOfBirth" },
        { CreateRequest([new ReferralAllergyRequest(new string('a', 201), "Itching")]), "Patient.Allergies[0].Substance" },
        { CreateRequest([new ReferralAllergyRequest("Latex", new string('a', 501))]), "Patient.Allergies[0].Reaction" },
        { CreateRequest([], reason: new string('a', 4001)), "Reason" }
    };

    [Theory]
    [MemberData(nameof(InvalidRequests))]
    public async Task InvalidReferralValues_ReturnsValidationProblemAndStoresNothing(ReferralRequest request, string expectedErrorKey)
    {
        var client = CreateIdentifiedClient();

        var response = await client.PostAsJsonAsync("/api/referrals", request);

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
        var client = CreateIdentifiedClient();

        var response = await client.PostAsync("/api/referrals", new StringContent("{ not json", Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(Database.Referrals);
    }

    private static ReferralRequest CreateRequest(
        IReadOnlyList<ReferralAllergyRequest> allergies,
        string name = "Jan Jansen",
        DateOnly? dateOfBirth = null,
        string reason = "Discharged after hip surgery.") =>
        new(new ReferralPatientRequest(Bsn, name, dateOfBirth ?? DateOfBirth, allergies), reason);
}
