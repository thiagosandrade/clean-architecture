using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddCreatedUpdated : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "created_on",
            schema: "public",
            table: "users",
            type: "timestamp with time zone",
            nullable: false,
            defaultValueSql: "NOW()");

        migrationBuilder.AddColumn<DateTime>(
            name: "updated_on",
            schema: "public",
            table: "users",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "created_on",
            schema: "public",
            table: "user_permissions",
            type: "timestamp with time zone",
            nullable: false,
            defaultValueSql: "NOW()");

        migrationBuilder.AddColumn<DateTime>(
            name: "updated_on",
            schema: "public",
            table: "user_permissions",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "created_on",
            schema: "public",
            table: "todo_items",
            type: "timestamp with time zone",
            nullable: false,
            defaultValueSql: "NOW()");

        migrationBuilder.AddColumn<DateTime>(
            name: "updated_on",
            schema: "public",
            table: "todo_items",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "created_on",
            schema: "public",
            table: "permissions",
            type: "timestamp with time zone",
            nullable: false,
            defaultValueSql: "NOW()");

        migrationBuilder.AddColumn<DateTime>(
            name: "updated_on",
            schema: "public",
            table: "permissions",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.Sql(@"
            UPDATE public.permissions
            SET created_on = NOW(),
                updated_on = NOW()
            WHERE id IN (
                '5bbb01f3-adbf-4fed-b5a6-70d3fe07da7d',
                'd5b5de09-34d0-4f34-8d60-410db716454b',
                'f7c8b043-d353-4a0a-8745-e0ce95a414ac'
            );
        ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "created_on",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "updated_on",
            schema: "public",
            table: "users");

        migrationBuilder.DropColumn(
            name: "created_on",
            schema: "public",
            table: "user_permissions");

        migrationBuilder.DropColumn(
            name: "updated_on",
            schema: "public",
            table: "user_permissions");

        migrationBuilder.DropColumn(
            name: "created_on",
            schema: "public",
            table: "todo_items");

        migrationBuilder.DropColumn(
            name: "updated_on",
            schema: "public",
            table: "todo_items");

        migrationBuilder.DropColumn(
            name: "created_on",
            schema: "public",
            table: "permissions");

        migrationBuilder.DropColumn(
            name: "updated_on",
            schema: "public",
            table: "permissions");
    }
}
