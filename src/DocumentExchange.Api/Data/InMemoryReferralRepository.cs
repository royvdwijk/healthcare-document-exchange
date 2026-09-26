using DocumentExchange.Api.Models;

namespace DocumentExchange.Api.Data;

/// <summary>Repository which manages referrals in the <see cref="InMemoryDatabase"/>.</summary>
public sealed class InMemoryReferralRepository(InMemoryDatabase database) : IReferralRepository
{
    public Task AddAsync(Referral referral, CancellationToken cancellationToken = default)
    {
        database.Referrals[referral.Id] = referral;
        return Task.CompletedTask;
    }
}
