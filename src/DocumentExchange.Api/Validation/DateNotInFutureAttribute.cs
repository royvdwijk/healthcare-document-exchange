using System.ComponentModel.DataAnnotations;

namespace DocumentExchange.Api.Validation;

/// <summary>
/// Validates that a <see cref="DateOnly"/> is not after today, using UTC comparison.
/// </summary>
/// <remarks>
/// A <see langword="null"/> value is valid, add <see cref="RequiredAttribute"/> when the date is required.
/// Using this on anything other than a <see cref="DateOnly"/> throws an <see cref="InvalidOperationException"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class DateNotInFutureAttribute() : ValidationAttribute("The date cannot be in the future.")
{
    public override bool IsValid(object? value) => value switch
    {
        null => true,
        DateOnly date => date <= DateOnly.FromDateTime(DateTime.UtcNow),
        _ => throw new InvalidOperationException($"{nameof(DateNotInFutureAttribute)} only supports {nameof(DateOnly)} values.")
    };
}
