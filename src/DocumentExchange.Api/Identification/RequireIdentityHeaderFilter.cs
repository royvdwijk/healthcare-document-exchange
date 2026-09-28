namespace DocumentExchange.Api.Identification;

/// <summary>
/// Rejects requests without the <see cref="IdentityHeader"/>.
/// </summary>
public sealed class RequireIdentityHeaderFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (IdentityHeader.Find(context.HttpContext.Request) is null)
        {
            var errors = new Dictionary<string, string[]>
            {
                [IdentityHeader.Name] = [$"The '{IdentityHeader.Name}' header is required and states who is making the request."]
            };

            return TypedResults.ValidationProblem(errors);
        }

        return await next(context);
    }
}
