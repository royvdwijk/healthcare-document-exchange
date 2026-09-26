using DocumentExchange.Api.Data;

namespace DocumentExchange.Api.Hosted;

/// <summary>Seeds the database with <see cref="SeedData"/> on startup.</summary>
/// <remarks>Seeding is for development environments and should not be registered when this is not the case.</remarks>
public sealed class DevPatientSeeder(IPatientRepository patients, ILogger<DevPatientSeeder> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var patient in SeedData.Patients)
        {
            await patients.AddAsync(patient, stoppingToken);
        }

        logger.LogInformation("Seeded {PatientCount} patient(s)", SeedData.Patients.Count);
    }
}
