namespace Auth.Objects;

public class TokenRequest
{
    public string? Email { get; init; }

    public string? Password { get; init; }
}