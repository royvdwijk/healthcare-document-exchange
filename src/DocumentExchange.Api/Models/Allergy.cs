namespace DocumentExchange.Api.Models;

/// <summary>
/// Represents a related allergy of an <see cref="Patient"/> object.
/// </summary>
/// <param name="Substance">The substance that triggers the allergy</param>
/// <param name="Reaction">The reaction to the <paramref name="Substance"/> when in contact.</param>
public sealed record Allergy(string Substance, string Reaction);
