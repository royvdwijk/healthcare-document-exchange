namespace DocumentExchange.Api.Hosted;

public static class HostedServiceExtensions
{
    /// <summary>Registers an instance of <see cref="DevPatientSeeder"/> which provides the application with test data.</summary>
    /// <remarks>This is for development environments and should not be registered when this is not the case.</remarks>
    public static IServiceCollection AddDevPatientSeeder(this IServiceCollection services) =>
        services.AddHostedService<DevPatientSeeder>();
}
