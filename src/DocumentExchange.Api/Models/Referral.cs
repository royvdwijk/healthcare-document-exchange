namespace DocumentExchange.Api.Models;

/// <summary>
/// Represents a received referral letter (verwijsbrief), including all information that was sent.
/// </summary>
/// <param name="Id">The unique identifier assigned when the referral was received.</param>
/// <param name="ReceivedAt">The moment the referral was received.</param>
/// <param name="Patient">The referred patient, including the allergies that were sent along.</param>
/// <param name="Reason">The reason for the referral.</param>
public sealed record Referral(
    Guid Id,
    DateTimeOffset ReceivedAt,
    Patient Patient,
    string Reason);
