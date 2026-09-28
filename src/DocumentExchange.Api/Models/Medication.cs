namespace DocumentExchange.Api.Models;

/// <summary>
/// Represents a medication used by a <see cref="Patient"/>.
/// </summary>
/// <param name="Name">The name of the medication.</param>
/// <param name="Dosage">The amount taken per use, e.g. <c>500 mg</c>.</param>
/// <param name="Frequency">How often the <paramref name="Dosage"/> is taken, e.g. <c>3 times a day</c>.</param>
public sealed record Medication(string Name, string Dosage, string Frequency);
