using DocumentExchange.Api.Models;

namespace DocumentExchange.Api.Data;

public interface IPatientRepository
{
    /// <summary>
    /// Adds a new patient instance to the repository.
    /// </summary>
    /// <param name="patient">The patient to add.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddAsync(Patient patient, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a patient by their unique BSN.
    /// </summary>
    /// <param name="bsn">The BSN to retrieve the patient by.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A single patient when found. When not found, <see langword="null"/>.</returns>
    Task<Patient?> GetByBsnAsync(string bsn, CancellationToken cancellationToken = default);
}
