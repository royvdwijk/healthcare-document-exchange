using System.ComponentModel.DataAnnotations;

namespace DocumentExchange.Api.Contracts;

/// <summary>
/// Represents an incoming referral request.
/// </summary>
/// <param name="Patient">The referred patient.</param>
/// <param name="Reason">The reason for the referral.</param>
public sealed record ReferralRequest([Required] ReferralPatientRequest Patient, [Required] string Reason);
