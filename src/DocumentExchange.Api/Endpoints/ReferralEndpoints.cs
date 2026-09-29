using DocumentExchange.Api.Contracts;
using DocumentExchange.Api.Data;
using DocumentExchange.Api.Identification;
using DocumentExchange.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DocumentExchange.Api.Endpoints;

public static class ReferralEndpoints
{
    /// <summary>Maps the referral endpoints.</summary>
    public static IEndpointRouteBuilder MapReferralEndpoints(this IEndpointRouteBuilder app)
    {
        var referrals = app.MapGroup("/api/referrals")
            .WithTags("Referrals")
            .AddEndpointFilter<RequireIdentityHeaderFilter>();

        referrals.MapPost("/", ReceiveReferral)
            .WithSummary("Receive a referral letter from another care provider.");

        return app;
    }

    private static async Task<Created<ReferralResponse>> ReceiveReferral(
        ReferralRequest request,
        IPatientRepository patients,
        IReferralRepository referrals,
        HttpRequest httpRequest,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger(typeof(ReferralEndpoints));
        var patient = request.Patient.ToPatient();

        var referral = new Referral(
            Id: Guid.NewGuid(),
            ReceivedAt: DateTimeOffset.UtcNow,
            Owner: IdentityHeader.Get(httpRequest),
            patient,
            request.Reason);

        await patients.AddOrMerge(patient, cancellationToken);
        await referrals.AddAsync(referral, cancellationToken);

        logger.LogInformation(
            "Received referral {ReferralId} from {Owner} for patient {Bsn} with {AllergyCount} allergies",
            referral.Id, referral.Owner, patient.Bsn, patient.Allergies.Count);

        return TypedResults.Created((string?)null, ReferralResponse.FromReferral(referral));
    }
}
