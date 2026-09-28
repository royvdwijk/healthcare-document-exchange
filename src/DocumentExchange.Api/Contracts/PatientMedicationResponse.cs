using DocumentExchange.Api.Models;

namespace DocumentExchange.Api.Contracts;

/// <summary>
/// Represents a medication of a patient response.
/// </summary>
/// <param name="Name">The name of the medication.</param>
/// <param name="Dosage">The amount taken per use.</param>
/// <param name="Frequency">How often the <paramref name="Dosage"/> is taken.</param>
public sealed record PatientMedicationResponse(string Name, string Dosage, string Frequency)
{
    /// <summary>Converts a <see cref="Medication"/> to a <see cref="PatientMedicationResponse"/>.</summary>
    public static PatientMedicationResponse FromMedication(Medication medication) =>
        new(medication.Name, medication.Dosage, medication.Frequency);
}
