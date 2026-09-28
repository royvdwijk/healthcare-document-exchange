namespace DocumentExchange.Api.Identification;

/// <summary>
/// The header in which the caller states who it is.
/// </summary>
public static class IdentityHeader
{
    public const string Name = "X-Identity";

    /// <summary>Gets the identity from the <paramref name="request"/>, or <see langword="null"/> when not provided.</summary>
    public static string? Find(HttpRequest request)
    {
        var identity = request.Headers[Name].ToString();
        return string.IsNullOrWhiteSpace(identity) ? null : identity;
    }

    /// <summary>Gets the identity from the <paramref name="request"/>.</summary>
    /// <remarks>Only use this on endpoints with the <see cref="RequireIdentityHeaderFilter"/>, which guarantees the header.</remarks>
    /// <exception cref="InvalidOperationException">Thrown when the header is not provided.</exception>
    public static string Get(HttpRequest request) =>
        Find(request) ?? throw new InvalidOperationException($"The '{Name}' header is not provided.");
}
