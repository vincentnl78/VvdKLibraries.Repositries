namespace Auth.Objects;

public class SlimUser
{
    public required string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public required string Id { get; init; }
}