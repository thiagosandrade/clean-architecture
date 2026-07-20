using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class RenameTaskEntities : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "task_activities",
            schema: "public");

        migrationBuilder.DropTable(
            name: "task_attachments",
            schema: "public");

        migrationBuilder.DropTable(
            name: "task_dependencies",
            schema: "public");

        migrationBuilder.CreateTable(
            name: "todo_activities",
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
                table.PrimaryKey("pk_todo_activities", x => x.id);
                table.ForeignKey(
                    name: "fk_todo_activities_todo_items_todo_item_id",
                    column: x => x.todo_item_id,
                    principalSchema: "public",
                    principalTable: "todo_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_todo_activities_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "public",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "todo_attachments",
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
                data = table.Column<byte[]>(type: "bytea", nullable: false),
                created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_todo_attachments", x => x.id);
                table.ForeignKey(
                    name: "fk_todo_attachments_todo_items_todo_item_id",
                    column: x => x.todo_item_id,
                    principalSchema: "public",
                    principalTable: "todo_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "todo_dependencies",
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
                table.PrimaryKey("pk_todo_dependencies", x => x.id);
                table.ForeignKey(
                    name: "fk_todo_dependencies_todo_items_depends_on_todo_item_id",
                    column: x => x.depends_on_todo_item_id,
                    principalSchema: "public",
                    principalTable: "todo_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_todo_dependencies_todo_items_todo_item_id",
                    column: x => x.todo_item_id,
                    principalSchema: "public",
                    principalTable: "todo_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_todo_activities_created_at_utc",
            schema: "public",
            table: "todo_activities",
            column: "created_at_utc");

        migrationBuilder.CreateIndex(
            name: "ix_todo_activities_todo_item_id",
            schema: "public",
            table: "todo_activities",
            column: "todo_item_id");

        migrationBuilder.CreateIndex(
            name: "ix_todo_activities_user_id",
            schema: "public",
            table: "todo_activities",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_todo_attachments_created_on",
            schema: "public",
            table: "todo_attachments",
            column: "created_on");

        migrationBuilder.CreateIndex(
            name: "ix_todo_attachments_todo_item_id",
            schema: "public",
            table: "todo_attachments",
            column: "todo_item_id");

        migrationBuilder.CreateIndex(
            name: "ix_todo_dependencies_depends_on_todo_item_id",
            schema: "public",
            table: "todo_dependencies",
            column: "depends_on_todo_item_id");

        migrationBuilder.CreateIndex(
            name: "ix_todo_dependencies_todo_item_id",
            schema: "public",
            table: "todo_dependencies",
            column: "todo_item_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "todo_activities",
            schema: "public");

        migrationBuilder.DropTable(
            name: "todo_attachments",
            schema: "public");

        migrationBuilder.DropTable(
            name: "todo_dependencies",
            schema: "public");

        migrationBuilder.CreateTable(
            name: "task_activities",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                todo_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", maxLength: 100, nullable: false),
                activity_type = table.Column<int>(type: "integer", nullable: false),
                created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
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

        migrationBuilder.CreateTable(
            name: "task_attachments",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                data = table.Column<byte[]>(type: "bytea", nullable: false),
                original_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                size = table.Column<long>(type: "bigint", nullable: false),
                stored_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                todo_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_task_attachments", x => x.id);
                table.ForeignKey(
                    name: "fk_task_attachments_todo_items_todo_item_id",
                    column: x => x.todo_item_id,
                    principalSchema: "public",
                    principalTable: "todo_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "task_dependencies",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                depends_on_todo_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                todo_item_id = table.Column<Guid>(type: "uuid", nullable: false),
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
            name: "ix_task_activities_created_at_utc",
            schema: "public",
            table: "task_activities",
            column: "created_at_utc");

        migrationBuilder.CreateIndex(
            name: "ix_task_activities_todo_item_id",
            schema: "public",
            table: "task_activities",
            column: "todo_item_id");

        migrationBuilder.CreateIndex(
            name: "ix_task_activities_user_id",
            schema: "public",
            table: "task_activities",
            column: "user_id");

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
}
