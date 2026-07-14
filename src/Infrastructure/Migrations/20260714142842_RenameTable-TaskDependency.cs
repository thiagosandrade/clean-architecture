using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class RenameTableTaskDependency : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameTable(
            name: "TaskActivities",
            schema: "public",
            newName: "task_activities",
            newSchema: "public");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameTable(
            name: "task_activities",
            schema: "public",
            newName: "TaskActivities",
            newSchema: "public");
    }
}
