using DocumentExchange.Api.Models;

namespace DocumentExchange.Api.Contracts;

/// <summary>
/// Represents an allergy of a patient response.
/// </summary>
/// <param name="Substance">The substance that triggers the allergy.</param>
/// <param name="Reaction">The reaction to the <paramref name="Substance"/> when in contact.</param>
public sealed record PatientAllergyResponse(string Substance, string Reaction)
{
    /// <summary>Converts an <see cref="Allergy"/> to a <see cref="PatientAllergyResponse"/>.</summary>
    public static PatientAllergyResponse FromAllergy(Allergy allergy) => new(allergy.Substance, allergy.Reaction);
}
