using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddTaskAttachment : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "task_attachments",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                todo_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                original_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                stored_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                size = table.Column<long>(type: "bigint", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_task_attachments", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_task_attachments_created_on",
            schema: "public",
            table: "task_attachments",
            column: "created_on");

        migrationBuilder.CreateIndex(
            name: "ix_task_attachments_todo_item_id",
            schema: "public",
            table: "task_attachments",
            column: "todo_item_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "task_attachments",
            schema: "public");
    }
}
