using System.Net;
using System.Net.Http.Json;
using DocumentExchange.Api.Contracts;
using DocumentExchange.Api.Identification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DocumentExchange.IntegrationTests;

[Collection(ApiCollection.Name)]
public class IdentityHeaderTests(WebApplicationFactory<Program> factory) : ApiCollectionTestBase(factory)
{
    private const string Bsn = "999990019";

    [Fact]
    public async Task Referral_WithoutIdentity_ReturnsValidationProblem()
    {
        var client = Factory.CreateClient();
        var request = new ReferralRequest(new ReferralPatientRequest(Bsn, "Jan Jansen", new DateOnly(1942, 3, 14), []), "Discharged.");

        var response = await client.PostAsJsonAsync("/api/referrals", request);

        await AssertIdentityRequiredAsync(response);
        Assert.Empty(Database.Referrals);
    }

    [Fact]
    public async Task Patient_WithoutIdentity_ReturnsValidationProblem()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync($"/api/patients/{Bsn}");

        await AssertIdentityRequiredAsync(response);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Patient_EmptyIdentity_ReturnsValidationProblem(string identity)
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add(IdentityHeader.Name, identity);

        var response = await client.GetAsync($"/api/patients/{Bsn}");

        await AssertIdentityRequiredAsync(response);
    }

    [Fact]
    public async Task Patient_WithIdentity_IsAccepted()
    {
        var client = CreateIdentifiedClient();

        var response = await client.GetAsync($"/api/patients/{Bsn}");

        // Identity is accepted, just no response.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static async Task AssertIdentityRequiredAsync(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains(IdentityHeader.Name, problem.Errors.Keys);
    }
}
