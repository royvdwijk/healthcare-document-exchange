using DocumentExchange.Api.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentExchange.IntegrationTests;

/// <summary>Base class for tests in the <see cref="ApiCollection"/>. Empties the database before each test.</summary>
public abstract class ApiCollectionTestBase(WebApplicationFactory<Program> factory) : IAsyncLifetime
{
    protected WebApplicationFactory<Program> Factory { get; } = factory;

    protected InMemoryDatabase Database => Factory.Services.GetRequiredService<InMemoryDatabase>();

    public Task InitializeAsync()
    {
        Database.Patients.Clear();
        Database.Referrals.Clear();
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
