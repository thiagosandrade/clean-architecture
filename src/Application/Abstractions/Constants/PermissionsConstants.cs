namespace Application.Abstractions.Constants;

public static class PermissionsConstants
{

    public static readonly Guid UsersAccessId = Guid.Parse("F7C8B043-D353-4A0A-8745-E0CE95A414AC");
    public static readonly Guid TodoAccessId = Guid.Parse("5BBB01F3-ADBF-4FED-B5A6-70D3FE07DA7D");
    public static readonly Guid PermissionAccessId = Guid.Parse("D5B5DE09-34D0-4F34-8D60-410DB716454B");
    
    public const string UsersAccess = "users:access";
    public const string TodoAccess = "todo:access";
    public const string PermissionAccess = "permission:access";
}
