namespace Application.Abstractions.Constants;

public static class PermissionsConstants
{

    public static readonly Guid UsersAccessId = Guid.Parse("F7C8B043-D353-4A0A-8745-E0CE95A414AC");
    public static readonly Guid TodoAccessId = Guid.Parse("5BBB01F3-ADBF-4FED-B5A6-70D3FE07DA7D");
    public static readonly Guid PermissionAccessId = Guid.Parse("D5B5DE09-34D0-4F34-8D60-410DB716454B");
    public static readonly Guid ActivityAccessId = Guid.Parse("1090B64B-1B68-4365-98B4-0E1F64E64F53");

    public const string UsersAccess = "users:access";
    public const string TodoAccess = "todo:access";
    public const string PermissionAccess = "permission:access";
    public const string ActivityAccess = "activity:access";
}
