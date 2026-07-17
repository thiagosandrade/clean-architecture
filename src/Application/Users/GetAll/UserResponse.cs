namespace Application.Users.GetAll;

public sealed record UserResponse
{
    public Guid Id { get; init; }

    public string Email { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }

    public DateTime CreatedOn { get; set; }

    public List<PermissionResponse> Permissions { get; init; }
}

public sealed record PermissionResponse
{
    public Guid Id { get; set; }
    public Guid PermissionId { get; set; }
    public Guid UserId { get; set; }
    public string Description { get; set; }
}
