using System.Diagnostics.CodeAnalysis;
using DocumentExchange.Api.Contracts;

namespace DocumentExchange.Api.Utils;

/// <summary>
/// Parses the <c>include</c> query values of the patient endpoint to <see cref="PatientInclude"/>.
/// </summary>
public static class PatientIncludeParser
{
    /// <summary>Parses the <paramref name="value"/> by name, ignoring casing and ensuring distinction.</summary>
    public static bool TryParseName(string value, out PatientInclude include)
    {
        include = default;
        return Enum.GetNames<PatientInclude>().Contains(value, StringComparer.OrdinalIgnoreCase)
            && Enum.TryParse(value, ignoreCase: true, out include);
    }

    /// <summary>Parses all <paramref name="values"/> by name, ignoring casing and ensuring distinction.</summary>
    /// <param name="values">The requested include values.</param>
    /// <param name="includes">The parsed includes, when all values are valid.</param>
    /// <param name="invalidValue">The first value that is not valid.</param>
    public static bool TryParseNames(
        IEnumerable<string> values,
        [NotNullWhen(true)] out HashSet<PatientInclude>? includes,
        [NotNullWhen(false)] out string? invalidValue)
    {
        var parsedIncludes = new HashSet<PatientInclude>();

        foreach (var value in values)
        {
            if (!TryParseName(value, out var include))
            {
                includes = null;
                invalidValue = value;
                return false;
            }

            parsedIncludes.Add(include);
        }

        includes = parsedIncludes;
        invalidValue = null;
        return true;
    }
}
