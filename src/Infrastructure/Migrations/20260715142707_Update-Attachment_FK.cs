using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdateAttachment_FK : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddForeignKey(
            name: "fk_task_attachments_todo_items_todo_item_id",
            schema: "public",
            table: "task_attachments",
            column: "todo_item_id",
            principalSchema: "public",
            principalTable: "todo_items",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_task_attachments_todo_items_todo_item_id",
            schema: "public",
            table: "task_attachments");
    }
}
