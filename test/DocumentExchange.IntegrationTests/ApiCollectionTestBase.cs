using DocumentExchange.Api.Data;
using DocumentExchange.Api.Identification;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentExchange.IntegrationTests;

/// <summary>Base class for tests in the <see cref="ApiCollection"/>. Empties the database before each test.</summary>
public abstract class ApiCollectionTestBase(WebApplicationFactory<Program> factory) : IAsyncLifetime
{
    protected WebApplicationFactory<Program> Factory { get; } = factory;

    protected InMemoryDatabase Database => Factory.Services.GetRequiredService<InMemoryDatabase>();

    /// <summary>The identity the client from <see cref="CreateIdentifiedClient"/> states.</summary>
    protected const string Identity = "Test";

    /// <summary>Creates a client that states who it is with the <see cref="IdentityHeader"/>.</summary>
    protected HttpClient CreateIdentifiedClient()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add(IdentityHeader.Name, Identity);
        return client;
    }

    public Task InitializeAsync()
    {
        Database.Patients.Clear();
        Database.Referrals.Clear();
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
