namespace Auth.Objects;

public class PasswordRequirements
{
    public bool RequireDigit { get; init; } = true;

    public bool RequireLowercase { get; init; } = true;

    public int RequiredLength { get; init; } = 8;

    public int MaxLength { get; init; } = 25;
    public bool RequireNonAlphanumeric { get; init; } = true;
    public bool RequireUppercase { get; init; } = true;
}