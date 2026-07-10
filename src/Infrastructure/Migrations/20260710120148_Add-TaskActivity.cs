using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddTaskActivity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TaskActivities",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                todo_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                activity_type = table.Column<int>(type: "integer", nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                user_id = table.Column<Guid>(type: "uuid", maxLength: 100, nullable: false),
                created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                metadata = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_task_activities", x => x.id);
                table.ForeignKey(
                    name: "fk_task_activities_todo_items_todo_item_id",
                    column: x => x.todo_item_id,
                    principalSchema: "public",
                    principalTable: "todo_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_task_activities_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "public",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_task_activities_created_at_utc",
            schema: "public",
            table: "TaskActivities",
            column: "created_at_utc");

        migrationBuilder.CreateIndex(
            name: "ix_task_activities_todo_item_id",
            schema: "public",
            table: "TaskActivities",
            column: "todo_item_id");

        migrationBuilder.CreateIndex(
            name: "ix_task_activities_user_id",
            schema: "public",
            table: "TaskActivities",
            column: "user_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "TaskActivities",
            schema: "public");
    }
}
