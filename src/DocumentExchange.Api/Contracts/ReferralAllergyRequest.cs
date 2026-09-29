using System.ComponentModel.DataAnnotations;
using DocumentExchange.Api.Models;

namespace DocumentExchange.Api.Contracts;

/// <summary>
/// Represents an allergy as part of a referral request.
/// </summary>
/// <param name="Substance">The substance that triggers the allergy.</param>
/// <param name="Reaction">The reaction to the <paramref name="Substance"/> when in contact.</param>
public sealed record ReferralAllergyRequest([Required, MaxLength(200)] string Substance, [Required, MaxLength(500)] string Reaction)
{
    /// <summary>Converts this request to an <see cref="Allergy"/>.</summary>
    public Allergy ToAllergy() => new(Substance, Reaction);
}
