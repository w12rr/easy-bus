namespace SharedCore.Infrastructure.Identity.ClientCredentials;

public sealed class IdentityProviderOptions
{
    public const string SectionName = "IdentityProvider";

    public required string Authority { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string ClientScope { get; init; }
    
    public string GetNormalizedAuthority() => Authority.TrimEnd('/');
}