using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class AdjustTodoActivityTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "created_at_utc",
            schema: "public",
            table: "todo_activities",
            newName: "created_on");

        migrationBuilder.RenameIndex(
            name: "ix_todo_activities_created_at_utc",
            schema: "public",
            table: "todo_activities",
            newName: "ix_todo_activities_created_on");

        migrationBuilder.AddColumn<DateTime>(
            name: "updated_on",
            schema: "public",
            table: "todo_activities",
            type: "timestamp with time zone",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "updated_on",
            schema: "public",
            table: "todo_activities");

        migrationBuilder.RenameColumn(
            name: "created_on",
            schema: "public",
            table: "todo_activities",
            newName: "created_at_utc");

        migrationBuilder.RenameIndex(
            name: "ix_todo_activities_created_on",
            schema: "public",
            table: "todo_activities",
            newName: "ix_todo_activities_created_at_utc");
    }
}
