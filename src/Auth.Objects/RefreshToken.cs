using System.Text.Json.Serialization;

namespace Auth.Objects;

public record RefreshToken
{
    [JsonIgnore] public string ResultCodeText => ResultCode.ToString();

    [JsonIgnore] public JwtTokenResult.AuthenticationErrorCodes ResultCode { get; init; }

    public required string UserId { get; init; }
    public required string Token { get; init; }
    public DateTimeOffset Expiration { get; init; }
    public required string Id { get; init; }
}