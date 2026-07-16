using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddDefaultUser : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            schema: "public",
            table: "users",
            columns: ["id", "created_on", "email", "first_name", "last_name", "password_hash", "updated_on"],
            values: new object[] { new Guid("d49c37cb-4774-4399-9848-63f89710491f"), new DateTime(2026, 7, 16, 13, 25, 57, 0, DateTimeKind.Utc), "james.bond@mail.com", "james", "bond", "646584747216FCD2E04A09CB831A495213422447104F0DAE7103ACAAB05AD988-3DF7AD47EDD6E4B714EA6898AF9A6B68", null });

        migrationBuilder.InsertData(
            schema: "public",
            table: "user_permissions",
            columns: ["id", "created_on", "permission_id", "updated_on", "user_id"],
            values: new object[,]
            {
                { new Guid("019f6b1a-f9a3-7ff2-bb54-9223cdf5d34f"), new DateTime(2026, 7, 16, 13, 25, 57, 0, DateTimeKind.Utc), new Guid("d5b5de09-34d0-4f34-8d60-410db716454b"), null, new Guid("d49c37cb-4774-4399-9848-63f89710491f") },
                { new Guid("019f6b1c-a541-7310-a652-021d00ca5fe8"), new DateTime(2026, 7, 16, 13, 27, 47, 0, DateTimeKind.Utc), new Guid("5bbb01f3-adbf-4fed-b5a6-70d3fe07da7d"), null, new Guid("d49c37cb-4774-4399-9848-63f89710491f") },
                { new Guid("019f6b1d-6e35-71e4-a136-37cd0f95ad0d"), new DateTime(2026, 7, 16, 13, 28, 38, 0, DateTimeKind.Utc), new Guid("1090b64b-1b68-4365-98b4-0e1f64e64f53"), null, new Guid("d49c37cb-4774-4399-9848-63f89710491f") },
                { new Guid("019f6b1d-db36-7d5a-ba4b-9be1aca1137b"), new DateTime(2026, 7, 16, 13, 29, 6, 0, DateTimeKind.Utc), new Guid("f7c8b043-d353-4a0a-8745-e0ce95a414ac"), null, new Guid("d49c37cb-4774-4399-9848-63f89710491f") }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            schema: "public",
            table: "user_permissions",
            keyColumn: "id",
            keyValue: new Guid("019f6b1a-f9a3-7ff2-bb54-9223cdf5d34f"));

        migrationBuilder.DeleteData(
            schema: "public",
            table: "user_permissions",
            keyColumn: "id",
            keyValue: new Guid("019f6b1c-a541-7310-a652-021d00ca5fe8"));

        migrationBuilder.DeleteData(
            schema: "public",
            table: "user_permissions",
            keyColumn: "id",
            keyValue: new Guid("019f6b1d-6e35-71e4-a136-37cd0f95ad0d"));

        migrationBuilder.DeleteData(
            schema: "public",
            table: "user_permissions",
            keyColumn: "id",
            keyValue: new Guid("019f6b1d-db36-7d5a-ba4b-9be1aca1137b"));

        migrationBuilder.DeleteData(
            schema: "public",
            table: "users",
            keyColumn: "id",
            keyValue: new Guid("d49c37cb-4774-4399-9848-63f89710491f"));
    }
}
