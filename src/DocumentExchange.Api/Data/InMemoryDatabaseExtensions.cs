using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DocumentExchange.Api.Data;

public static class InMemoryDatabaseExtensions
{
    /// <summary>Registers the patient repository, backed by an <see cref="InMemoryDatabase"/> instance shared across the application.</summary>
    public static IServiceCollection AddInMemoryPatientDatabase(this IServiceCollection services)
    {
        // Only adds the database when another extension has not added it already.
        services.TryAddSingleton<InMemoryDatabase>();

        services.AddSingleton<IPatientRepository, InMemoryPatientRepository>();
        return services;
    }

    /// <summary>Registers the referral repository, backed by an <see cref="InMemoryDatabase"/> instance shared across the application.</summary>
    public static IServiceCollection AddInMemoryReferralDatabase(this IServiceCollection services)
    {
        // Only adds the database when another extension has not added it already.
        services.TryAddSingleton<InMemoryDatabase>();

        services.AddSingleton<IReferralRepository, InMemoryReferralRepository>();
        return services;
    }
}
