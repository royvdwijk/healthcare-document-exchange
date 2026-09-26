using DocumentExchange.Api.Contracts;
using DocumentExchange.Api.Data;
using DocumentExchange.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DocumentExchange.Api.Endpoints;

public static class ReferralEndpoints
{
    /// <summary>Maps the referral endpoints.</summary>
    public static IEndpointRouteBuilder MapReferralEndpoints(this IEndpointRouteBuilder app)
    {
        var referrals = app.MapGroup("/api/referrals")
            .WithTags("Referrals");

        referrals.MapPost("/", ReceiveReferral)
            .WithSummary("Receive a referral letter from another care provider.");

        return app;
    }

    private static async Task<Created<Referral>> ReceiveReferral(
        ReferralRequest request,
        IPatientRepository patients,
        IReferralRepository referrals,
        CancellationToken cancellationToken)
    {
        var referral = new Referral(
            Id: Guid.NewGuid(),
            ReceivedAt: DateTimeOffset.UtcNow,
            request.Patient,
            request.Reason);

        await patients.AddOrMerge(request.Patient, cancellationToken);
        await referrals.AddAsync(referral, cancellationToken);

        return TypedResults.Created((string?)null, referral);
    }
}
