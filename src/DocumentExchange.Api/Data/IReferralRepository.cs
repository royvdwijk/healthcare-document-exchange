using DocumentExchange.Api.Models;

namespace DocumentExchange.Api.Data;

public interface IReferralRepository
{
    /// <summary>
    /// Adds a received referral to the repository.
    /// </summary>
    /// <param name="referral">The referral to add.</param>
    /// <param name="cancellationToken"></param>
    Task AddAsync(Referral referral, CancellationToken cancellationToken = default);
}
