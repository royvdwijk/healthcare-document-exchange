using DocumentExchange.Api.Contracts;
using DocumentExchange.Api.Data;
using DocumentExchange.Api.Identification;
using DocumentExchange.Api.Utils;
using DocumentExchange.Api.Validation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DocumentExchange.Api.Endpoints;

public static class PatientEndpoints
{
    /// <summary>Maps the patient endpoints.</summary>
    public static IEndpointRouteBuilder MapPatientEndpoints(this IEndpointRouteBuilder app)
    {
        var patients = app.MapGroup("/api/patients")
            .WithTags("Patients")
            .AddEndpointFilter<RequireIdentityHeaderFilter>();

        patients.MapGet("/{bsn}", GetPatient)
            .WithSummary("Get information about a patient, optionally including extra information such as allergies and medications.");

        return app;
    }

    private static async Task<Results<Ok<PatientResponse>, NotFound, ValidationProblem>> GetPatient(
        [Bsn] string bsn,
        string[]? include,
        IPatientRepository patients,
        HttpRequest httpRequest,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger(typeof(PatientEndpoints));
        var identity = IdentityHeader.Get(httpRequest);

        if (!PatientIncludeParser.TryParseNames(include ?? [], out var includes, out var invalidValue))
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
            logger.LogWarning("Patient {Bsn} was requested by {Identity} but is not known", bsn, identity);
            return TypedResults.NotFound();
        }

        var response = PatientResponse.FromPatient(
            patient,
            includeAllergies: includes.Contains(PatientInclude.Allergies),
            includeMedications: includes.Contains(PatientInclude.Medications));

        logger.LogInformation(
            "Shared patient {Bsn} with {Identity} including {Includes}: {AllergyCount} allergies, {MedicationCount} medications",
            patient.Bsn, identity, includes, response.Allergies?.Count ?? 0, response.Medications?.Count ?? 0);
        return TypedResults.Ok(response);
    }
}
