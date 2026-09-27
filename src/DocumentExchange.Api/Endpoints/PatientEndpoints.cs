using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DocumentExchange.Api.Contracts;
using DocumentExchange.Api.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DocumentExchange.Api.Endpoints;

public static class PatientEndpoints
{
    /// <summary>Maps the patient endpoints.</summary>
    public static IEndpointRouteBuilder MapPatientEndpoints(this IEndpointRouteBuilder app)
    {
        var patients = app.MapGroup("/api/patients")
            .WithTags("Patients");

        patients.MapGet("/{bsn}", GetPatient)
            .WithSummary("Get information about a patient, optionally including extra information such as allergies.");

        return app;
    }

    private static async Task<Results<Ok<PatientResponse>, NotFound, ValidationProblem>> GetPatient(
        [RegularExpression(@"^\d{9}$", ErrorMessage = "The BSN must consist of exactly 9 digits.")] string bsn,
        string[]? include,
        IPatientRepository patients,
        CancellationToken cancellationToken)
    {
        if (!TryParseIncludes(include ?? [], out var includes, out var invalidValue))
        {
            var errors = new Dictionary<string, string[]>
            {
                ["include"] = [$"'{invalidValue}' is not a valid value. Valid values are: {string.Join(", ", Enum.GetNames<PatientInclude>())}."]
            };

            return TypedResults.ValidationProblem(errors);
        }

        var patient = await patients.GetByBsnAsync(bsn, cancellationToken);
        if (patient is null)
        {
            return TypedResults.NotFound();
        }

        var response = PatientResponse.FromPatient(patient, includes.Contains(PatientInclude.Allergies));
        return TypedResults.Ok(response);
    }


    /// <summary>Converts the <paramref name="values"/> to <see cref="PatientInclude"/>, ignoring casing.</summary>
    /// <param name="values"></param>
    /// <param name="includes"></param>
    /// <param name="invalidValue"></param>
    /// <returns>
    /// <see langword="true"/> with <paramref name="includes"/> when all values are valid.
    /// <br />
    /// <see langword="false"/> with <paramref name="invalidValue"/> when a value is invalid.
    /// </returns>
    private static bool TryParseIncludes(
        string[] values,
        [NotNullWhen(true)] out HashSet<PatientInclude>? includes,
        [NotNullWhen(false)] out string? invalidValue)
    {
        var parsedIncludes = new HashSet<PatientInclude>();

        foreach (var value in values)
        {
            if (!Enum.TryParse<PatientInclude>(value, ignoreCase: true, out var parsed))
            {
                includes = null;
                invalidValue = value;
                return false;
            }

            parsedIncludes.Add(parsed);
        }

        includes = parsedIncludes;
        invalidValue = null;
        return true;
    }
}
