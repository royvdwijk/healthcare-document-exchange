using DocumentExchange.Api.Models;

namespace DocumentExchange.Api.Contracts;

/// <summary>
/// Represents a patient response.
/// </summary>
/// <param name="Bsn">The BSN of the patient.</param>
/// <param name="Name">The name of the patient.</param>
/// <param name="DateOfBirth">The date of birth of the patient.</param>
/// <param name="Allergies">Allergies of the patient, or <see langword="null"/> when not part of the response.</param>
/// <param name="Medications">Medications of the patient, or <see langword="null"/> when not part of the response.</param>
public sealed record PatientResponse(
    string Bsn,
    string Name,
    DateOnly DateOfBirth,
    IReadOnlyList<PatientAllergyResponse>? Allergies,
    IReadOnlyList<PatientMedicationResponse>? Medications)
{
    /// <summary>Converts a <see cref="Patient"/> model to a response.</summary>
    /// <param name="patient">The patient to convert.</param>
    /// <param name="includeAllergies">Whether the allergies of the patient should be included.</param>
    /// <param name="includeMedications">Whether the medications of the patient should be included.</param>
    public static PatientResponse FromPatient(Patient patient, bool includeAllergies, bool includeMedications) => new(
        patient.Bsn,
        patient.Name,
        patient.DateOfBirth,
        includeAllergies ? [.. patient.Allergies.Select(PatientAllergyResponse.FromAllergy)] : null,
        includeMedications ? [.. patient.Medications.Select(PatientMedicationResponse.FromMedication)] : null);
}
