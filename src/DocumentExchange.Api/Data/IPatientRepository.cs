using DocumentExchange.Api.Models;

namespace DocumentExchange.Api.Data;

public interface IPatientRepository
{
    /// <summary>
    /// Adds or merges a <see cref="Patient"/> instance in the repository depending on whether the <paramref name="patient"/> instance is known, using equality.
    /// Information should be retained with a merge.
    /// </summary>
    /// <param name="patient">The patient to add or merge.</param>
    /// <param name="cancellationToken"></param>
    Task AddOrMerge(Patient patient, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a patient by their unique BSN.
    /// </summary>
    /// <param name="bsn">The BSN to retrieve the patient by.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A single patient when found. When not found, <see langword="null"/>.</returns>
    Task<Patient?> GetByBsnAsync(string bsn, CancellationToken cancellationToken = default);
}
