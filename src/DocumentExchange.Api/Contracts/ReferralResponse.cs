using DocumentExchange.Api.Models;

namespace DocumentExchange.Api.Contracts;

/// <summary>
/// Represents a received referral letter response.
/// </summary>
/// <param name="Id">The unique identifier assigned when the referral was received.</param>
/// <param name="ReceivedAt">The moment the referral was received.</param>
/// <param name="Owner">The identity of the party that sent the referral.</param>
/// <param name="Patient">The referred patient, as sent with the referral.</param>
/// <param name="Reason">The reason for the referral.</param>
public sealed record ReferralResponse(
    Guid Id,
    DateTimeOffset ReceivedAt,
    string Owner,
    ReferralPatientResponse Patient,
    string Reason)
{
    /// <summary>Converts a <see cref="Referral"/> to a <see cref="ReferralResponse"/>.</summary>
    public static ReferralResponse FromReferral(Referral referral) => new(
        referral.Id,
        referral.ReceivedAt,
        referral.Owner,
        ReferralPatientResponse.FromPatient(referral.Patient),
        referral.Reason);
}
