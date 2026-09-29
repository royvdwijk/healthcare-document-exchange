namespace DocumentExchange.Api.Models;

/// <summary>
/// Represents a registered patient.
/// </summary>
/// <param name="Bsn">The unique BSN that distinguishes the patient from other instances.</param>
/// <param name="Name">The name of the patient.</param>
/// <param name="DateOfBirth">The date of birth of the patient, or <see langword="null"/> when not known.</param>
/// <param name="Allergies">A <see cref="List{T}"/> of <see cref="Allergy"/> objects that represents allergies this patient has.</param>
/// <param name="Medications">A <see cref="List{T}"/> of <see cref="Medication"/> objects that represents medications this patient uses.</param>
public sealed record Patient(
    string Bsn,
    string Name,
    DateOnly? DateOfBirth,
    IReadOnlyList<Allergy> Allergies,
    IReadOnlyList<Medication> Medications)
{
    /// <summary>
    /// Updates this patient with new data from the provided patient.
    /// Data that is not in the provided patient is retained.
    /// </summary>
    /// <param name="other">The newer information about the same patient.</param>
    /// <returns>A new <see cref="Patient"/> containing the merged information.</returns>
    /// <exception cref="ArgumentException">Thrown when the patients do not match in equality.</exception>
    public Patient Merge(Patient other)
    {
        if (other != this)
            throw new ArgumentException(nameof(Merge) + " requires the patients to match in equality.", nameof(other));

        return other with
        {
            DateOfBirth = other.DateOfBirth ?? DateOfBirth,
            Allergies = [.. other.Allergies.UnionBy(Allergies, allergy => allergy.Substance, StringComparer.OrdinalIgnoreCase)],
            Medications = [.. other.Medications.UnionBy(Medications, medication => medication.Name, StringComparer.OrdinalIgnoreCase)]
        };
    }

    public bool Equals(Patient? other) => other is not null && Bsn == other.Bsn;

    public override int GetHashCode() => Bsn.GetHashCode();
}
