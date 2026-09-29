using System.ComponentModel.DataAnnotations;
using DocumentExchange.Api.Models;
using DocumentExchange.Api.Validation;

namespace DocumentExchange.Api.Contracts;

/// <summary>
/// Represents a patient as part of a referral request.
/// </summary>
/// <param name="Bsn">The BSN of the patient.</param>
/// <param name="Name">The name of the patient.</param>
/// <param name="DateOfBirth">The date of birth of the patient, or <see langword="null"/> when not known.</param>
/// <param name="Allergies">The allergies of the patient.</param>
public sealed record ReferralPatientRequest(
    [Required, Bsn] string Bsn,
    [Required, MaxLength(200)] string Name,
    [DateNotInFuture] DateOnly? DateOfBirth,
    [Required] IReadOnlyList<ReferralAllergyRequest> Allergies)
{
    /// <summary>Converts this request to a <see cref="Patient"/>.</summary>
    /// <remarks>A referral does not contain medications, so the patient has none. Known medications are kept when merging.</remarks>
    public Patient ToPatient() => new(Bsn, Name, DateOfBirth, [.. Allergies.Select(allergy => allergy.ToAllergy())], Medications: []);
}
