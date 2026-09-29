namespace DocumentExchange.Api.Utils;

/// <summary>
/// Checks whether a value is a BSN.
/// </summary>
public static class BsnValidator
{
    private static readonly int[] Weights = [9, 8, 7, 6, 5, 4, 3, 2, -1];

    /// <summary>Checks that <paramref name="bsn"/> consists of exactly 9 ASCII digits and passes the 11-check.</summary>
    public static bool IsValid(string bsn)
    {
        if (bsn.Length != Weights.Length || !bsn.All(char.IsAsciiDigit))
            return false;

        // 11-check: each digit is multiplied by its weight, the sum must be divisible by 11.
        var sum = bsn.Select((digit, index) => (digit - '0') * Weights[index]).Sum();
        return sum % 11 == 0;
    }
}
