namespace DocumentExchange.Api.Models;

/// <summary>
/// Represents a registered patient.
/// </summary>
/// <param name="Bsn">The unique Bsn that distincts the patient from other instances.</param>
/// <param name="Name">The name of the patient.</param>
/// <param name="DateOfBirth">The date of birth of the parient.</param>
/// <param name="Allergies">A <see cref="List{T}"/> of <see cref="Allergy"/> objects that represents allergies this patient has.</param>
public sealed record Patient(string Bsn, string Name, DateOnly DateOfBirth, IReadOnlyList<Allergy> Allergies);
