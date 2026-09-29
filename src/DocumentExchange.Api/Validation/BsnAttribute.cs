using System.ComponentModel.DataAnnotations;
using DocumentExchange.Api.Utils;

namespace DocumentExchange.Api.Validation;

/// <summary>
/// Validates that a <see cref="string"/> is a BSN, see <see cref="BsnValidator.IsValid"/>.
/// </summary>
/// <remarks>
/// A <see langword="null"/> value is valid, add <see cref="RequiredAttribute"/> to a request when the BSN is required.
/// Using this on anything other than a <see cref="string"/> throws an <see cref="InvalidOperationException"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class BsnAttribute() : ValidationAttribute("The BSN must consist of exactly 9 digits and pass the 11-check.")
{
    public override bool IsValid(object? value) => value switch
    {
        null => true,
        string bsn => BsnValidator.IsValid(bsn),
        _ => throw new InvalidOperationException($"{nameof(BsnAttribute)} only supports string values.")
    };
}
