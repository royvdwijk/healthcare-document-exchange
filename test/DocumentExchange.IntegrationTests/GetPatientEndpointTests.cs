using System.Net;
using System.Net.Http.Json;
using DocumentExchange.Api.Contracts;
using DocumentExchange.Api.Data;
using DocumentExchange.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentExchange.IntegrationTests;

[Collection(ApiCollection.Name)]
public class GetPatientEndpointTests(WebApplicationFactory<Program> factory) : ApiCollectionTestBase(factory)
{
    private const string Bsn = "999990019";
    private static readonly DateOnly DateOfBirth = new(1942, 3, 14);

    [Fact]
    public async Task KnownPatient_ReturnsPatientWithoutExtraInformationByDefault()
    {
        await AddPatientAsync([new Allergy("Latex", "Itching")], [new Medication("Metoprolol", "50 mg", "Once a day")]);
        var client = CreateIdentifiedClient();

        var response = await client.GetAsync($"/api/patients/{Bsn}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var patient = await response.Content.ReadFromJsonAsync<PatientResponse>();
        Assert.NotNull(patient);
        Assert.Equal(Bsn, patient.Bsn);
        Assert.Equal("Jan Jansen", patient.Name);
        Assert.Equal(DateOfBirth, patient.DateOfBirth);
        Assert.Null(patient.Allergies);
        Assert.Null(patient.Medications);
    }

    [Theory]
    [InlineData("allergies")]
    [InlineData("Allergies")]
    [InlineData("ALLERGIES")]
    public async Task IncludeAllergies_ReturnsPatientWithAllergies(string include)
    {
        await AddPatientAsync([new Allergy("Latex", "Itching"), new Allergy("Penicillin", "Skin rash")]);
        var client = CreateIdentifiedClient();

        var response = await client.GetAsync($"/api/patients/{Bsn}?include={include}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var patient = await response.Content.ReadFromJsonAsync<PatientResponse>();
        Assert.NotNull(patient?.Allergies);
        Assert.Equal(2, patient.Allergies.Count);
        Assert.Contains(new PatientAllergyResponse("Latex", "Itching"), patient.Allergies);
        Assert.Contains(new PatientAllergyResponse("Penicillin", "Skin rash"), patient.Allergies);
        Assert.Null(patient.Medications);
    }

    [Theory]
    [InlineData("medications")]
    [InlineData("Medications")]
    [InlineData("MEDICATIONS")]
    public async Task IncludeMedications_ReturnsPatientWithMedications(string include)
    {
        await AddPatientAsync(
            [new Allergy("Latex", "Itching")],
            [new Medication("Metoprolol", "50 mg", "Once a day"), new Medication("Omeprazole", "20 mg", "Once a day")]);
        var client = CreateIdentifiedClient();

        var response = await client.GetAsync($"/api/patients/{Bsn}?include={include}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var patient = await response.Content.ReadFromJsonAsync<PatientResponse>();
        Assert.NotNull(patient?.Medications);
        Assert.Equal(2, patient.Medications.Count);
        Assert.Contains(new PatientMedicationResponse("Metoprolol", "50 mg", "Once a day"), patient.Medications);
        Assert.Contains(new PatientMedicationResponse("Omeprazole", "20 mg", "Once a day"), patient.Medications);
        Assert.Null(patient.Allergies);
    }

    [Fact]
    public async Task IncludeAllergiesAndMedications_ReturnsPatientWithBoth()
    {
        await AddPatientAsync([new Allergy("Latex", "Itching")], [new Medication("Metoprolol", "50 mg", "Once a day")]);
        var client = CreateIdentifiedClient();

        var response = await client.GetAsync($"/api/patients/{Bsn}?include=allergies&include=medications");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var patient = await response.Content.ReadFromJsonAsync<PatientResponse>();
        Assert.NotNull(patient);
        Assert.Equal([new PatientAllergyResponse("Latex", "Itching")], patient.Allergies);
        Assert.Equal([new PatientMedicationResponse("Metoprolol", "50 mg", "Once a day")], patient.Medications);
    }

    [Fact]
    public async Task UnknownPatient_ReturnsNotFound()
    {
        var client = CreateIdentifiedClient();

        var response = await client.GetAsync($"/api/patients/{Bsn}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("12345678")]
    [InlineData("1234567890")]
    [InlineData("12345678a")]
    [InlineData("123456780")]
    [InlineData("９９９９９００１９")]
    public async Task InvalidBsn_ReturnsValidationProblem(string bsn)
    {
        var client = CreateIdentifiedClient();

        var response = await client.GetAsync($"/api/patients/{bsn}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains("bsn", problem.Errors.Keys);
    }

    [Theory]
    [InlineData("hobbies")]
    [InlineData("0")]
    [InlineData("5")]
    public async Task UnknownInclude_ReturnsValidationProblem(string include)
    {
        await AddPatientAsync([]);
        var client = CreateIdentifiedClient();

        var response = await client.GetAsync($"/api/patients/{Bsn}?include=allergies&include={include}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains("include", problem.Errors.Keys);
    }

    private Task AddPatientAsync(IReadOnlyList<Allergy> allergies, IReadOnlyList<Medication>? medications = null) =>
        Factory.Services.GetRequiredService<IPatientRepository>()
            .AddOrMerge(new Patient(Bsn, "Jan Jansen", DateOfBirth, allergies, medications ?? []));
}
