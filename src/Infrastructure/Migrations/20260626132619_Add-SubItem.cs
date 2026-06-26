using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddSubItem : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "created_at",
            schema: "public",
            table: "todo_items");

        migrationBuilder.CreateTable(
            name: "todo_sub_item",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                todo_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                description = table.Column<string>(type: "text", nullable: false),
                is_completed = table.Column<bool>(type: "boolean", nullable: false),
                completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                order = table.Column<int>(type: "integer", nullable: false),
                created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_todo_sub_item", x => x.id);
                table.ForeignKey(
                    name: "fk_todo_sub_item_todo_items_todo_item_id",
                    column: x => x.todo_item_id,
                    principalSchema: "public",
                    principalTable: "todo_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_todo_sub_item_todo_item_id",
            schema: "public",
            table: "todo_sub_item",
            column: "todo_item_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "todo_sub_item",
            schema: "public");

        migrationBuilder.AddColumn<DateTime>(
            name: "created_at",
            schema: "public",
            table: "todo_items",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
    }
}
