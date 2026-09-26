using Microsoft.AspNetCore.Mvc.Testing;

namespace DocumentExchange.IntegrationTests;

/// <summary>Shares a single in-memory instance of the API between all tests in this collection.</summary>
[CollectionDefinition(Name)]
public class ApiCollection : ICollectionFixture<WebApplicationFactory<Program>>
{
    public const string Name = "Api";
}
