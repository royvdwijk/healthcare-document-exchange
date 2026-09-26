using DocumentExchange.Api.Models;

namespace DocumentExchange.Api.Data;

/// <summary>Repository which manages patients in the <see cref="InMemoryDatabase"/>.</summary>
public sealed class InMemoryPatientRepository(InMemoryDatabase database) : IPatientRepository
{
    public Task AddOrMerge(Patient patient, CancellationToken cancellationToken = default)
    {
        database.Patients.AddOrUpdate(patient.Bsn, patient, (_, existing) => existing.Merge(patient));
        return Task.CompletedTask;
    }

    public Task<Patient?> GetByBsnAsync(string bsn, CancellationToken cancellationToken = default) =>
        Task.FromResult(database.Patients.GetValueOrDefault(bsn));
}
