namespace Application.Permissions.GetAll;

public sealed record PermissionResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; }
}
