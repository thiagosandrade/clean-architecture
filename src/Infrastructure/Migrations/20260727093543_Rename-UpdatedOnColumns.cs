using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class RenameUpdatedOnColumns : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "completed_at",
            schema: "public",
            table: "todo_sub_items",
            newName: "completed_on");

        migrationBuilder.RenameColumn(
            name: "completed_at",
            schema: "public",
            table: "todo_items",
            newName: "completed_on");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "completed_on",
            schema: "public",
            table: "todo_sub_items",
            newName: "completed_at");

        migrationBuilder.RenameColumn(
            name: "completed_on",
            schema: "public",
            table: "todo_items",
            newName: "completed_at");
    }
}
