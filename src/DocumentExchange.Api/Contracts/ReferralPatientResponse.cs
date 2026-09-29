using DocumentExchange.Api.Models;

namespace DocumentExchange.Api.Contracts;

/// <summary>
/// Represents a patient as part of a referral response, as it was sent with the referral.
/// </summary>
/// <param name="Bsn">The BSN of the patient.</param>
/// <param name="Name">The name of the patient.</param>
/// <param name="DateOfBirth">The date of birth of the patient, or <see langword="null"/> when not known.</param>
/// <param name="Allergies">The allergies of the patient that were sent along.</param>
public sealed record ReferralPatientResponse(
    string Bsn,
    string Name,
    DateOnly? DateOfBirth,
    IReadOnlyList<PatientAllergyResponse> Allergies)
{
    /// <summary>Converts a <see cref="Patient"/> to a <see cref="ReferralPatientResponse"/>.</summary>
    /// <remarks>A referral does not contain medications, so they are not part of the response.</remarks>
    public static ReferralPatientResponse FromPatient(Patient patient) => new(
        patient.Bsn,
        patient.Name,
        patient.DateOfBirth,
        [.. patient.Allergies.Select(PatientAllergyResponse.FromAllergy)]);
}
