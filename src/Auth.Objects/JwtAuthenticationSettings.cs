namespace Auth.Objects;

public record JwtAuthenticationSettings
{
    public string Key { get; init; } = string.Empty;
    public double RefreshTokenExpirationMinutes { get; init; }
    public string SiteUrl { get; init; } = null!;
    public double TokenExpirationMinutes { get; init; }
}