using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddTaskActivityPermission : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            schema: "public",
            table: "permissions",
            columns: ["id", "description", "updated_on"],
            values: new object[] { new Guid("1090b64b-1b68-4365-98b4-0e1f64e64f53"), "activity:access", null });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            schema: "public",
            table: "permissions",
            keyColumn: "id",
            keyValue: new Guid("1090b64b-1b68-4365-98b4-0e1f64e64f53"));
    }
}
