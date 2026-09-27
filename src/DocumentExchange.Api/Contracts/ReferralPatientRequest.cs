using System.ComponentModel.DataAnnotations;
using DocumentExchange.Api.Models;

namespace DocumentExchange.Api.Contracts;

/// <summary>
/// Represents a patient as part of a referral request.
/// </summary>
/// <param name="Bsn">The BSN of the patient.</param>
/// <param name="Name">The name of the patient.</param>
/// <param name="DateOfBirth">The date of birth of the patient.</param>
/// <param name="Allergies">The allergies of the patient.</param>
public sealed record ReferralPatientRequest(
    [Required, RegularExpression(@"^\d{9}$", ErrorMessage = "The BSN must consist of exactly 9 digits.")] string Bsn,
    [Required] string Name,
    DateOnly DateOfBirth,
    [Required] IReadOnlyList<ReferralAllergyRequest> Allergies)
{
    /// <summary>Converts this request to a <see cref="Patient"/>.</summary>
    public Patient ToPatient() => new(Bsn, Name, DateOfBirth, [.. Allergies.Select(allergy => allergy.ToAllergy())]);
}
