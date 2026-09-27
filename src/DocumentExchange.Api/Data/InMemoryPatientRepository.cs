using DocumentExchange.Api.Models;

namespace DocumentExchange.Api.Data;

/// <summary>Repository which manages patients in the <see cref="InMemoryDatabase"/>.</summary>
public sealed class InMemoryPatientRepository(InMemoryDatabase database, ILogger<InMemoryPatientRepository> logger) : IPatientRepository
{
    public Task AddOrMerge(Patient patient, CancellationToken cancellationToken = default)
    {
        database.Patients.AddOrUpdate(patient.Bsn, patient, (_, existing) => existing.Merge(patient));

        // No clean way to log if we either added or merged, so just log both for now.
        logger.LogInformation("Added / merged patient {Bsn}", patient.Bsn);

        return Task.CompletedTask;
    }

    public Task<Patient?> GetByBsnAsync(string bsn, CancellationToken cancellationToken = default) =>
        Task.FromResult(database.Patients.GetValueOrDefault(bsn));
}
