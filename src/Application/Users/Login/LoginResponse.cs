namespace Application.Users.Login;

public sealed record LoginResponse
{
    public string Token { get; init; }
    public Guid Id { get; set; }
    public string Email { get; set; }
}
