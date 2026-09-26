using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DocumentExchange.Api.Data;

public static class InMemoryDatabaseExtensions
{
    /// <summary>Registers the patient repository, backed by a <see cref="InMemoryDatabase"/> instance shared accross the application.</summary>
    public static IServiceCollection AddInMemoryPatientDatabase(this IServiceCollection services)
    {
        // Only attempt to add the database in case another extension takes care of it.
        services.TryAddSingleton<InMemoryDatabase>();

        services.AddSingleton<IPatientRepository, InMemoryPatientRepository>();
        return services;
    }
}
