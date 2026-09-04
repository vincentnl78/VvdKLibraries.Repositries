using System.Text.Json.Serialization;

namespace Auth.Objects;

public class JwtToken
{
    public const string ApiBackEndUrlClaimIdentifier = "ApiUrl";
    public required DateTime Expiration { get; init; }
    public required string Token { get; init; }
    public required string RefreshToken { get; set; }
}

public class JwtTokenResult
{
    public enum AuthenticationErrorCodes
    {
        Ok,
        Expired, //only for refresh token
        InvalidLogin, //not confirmed or invalid password or does not exist
        InvalidRequest, //invalid request format
        ServerError //Server not available or server error will be in ApiResult on the communication level
    }

    public JwtTokenResult()
    {
    }

    public JwtTokenResult(AuthenticationErrorCodes resultCode, string errorMessage)
    {
        ResultCode = resultCode;
        ErrorMessage = errorMessage;
    }

    public JwtTokenResult(
        string token,
        string refreshToken,
        DateTime expiration
    )
    {
        Token = new JwtToken
        {
            Expiration = expiration,
            Token = token,
            RefreshToken = refreshToken
        };
    }

    [JsonIgnore] public string ResultCodeText => ResultCode.ToString();

    public AuthenticationErrorCodes ResultCode { get; init; } = AuthenticationErrorCodes.Ok;
    public string? ErrorMessage { get; init; } = string.Empty;
    public required string UserId { get; init; }
    public JwtToken? Token { get; init; }
}