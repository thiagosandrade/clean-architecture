using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddTaskDependency : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "task_dependencies",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                todo_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                depends_on_todo_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_task_dependencies", x => x.id);
                table.ForeignKey(
                    name: "fk_task_dependencies_todo_items_depends_on_todo_item_id",
                    column: x => x.depends_on_todo_item_id,
                    principalSchema: "public",
                    principalTable: "todo_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_task_dependencies_todo_items_todo_item_id",
                    column: x => x.todo_item_id,
                    principalSchema: "public",
                    principalTable: "todo_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_task_dependencies_depends_on_todo_item_id",
            schema: "public",
            table: "task_dependencies",
            column: "depends_on_todo_item_id");

        migrationBuilder.CreateIndex(
            name: "ix_task_dependencies_todo_item_id",
            schema: "public",
            table: "task_dependencies",
            column: "todo_item_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "task_dependencies",
            schema: "public");
    }
}
