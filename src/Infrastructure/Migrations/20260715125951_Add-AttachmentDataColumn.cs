using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddAttachmentDataColumn : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<byte[]>(
            name: "data",
            schema: "public",
            table: "task_attachments",
            type: "bytea",
            nullable: false,
            defaultValue: Array.Empty<byte>());
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "data",
            schema: "public",
            table: "task_attachments");
    }
}
