using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddSubItem2 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_todo_sub_item_todo_items_todo_item_id",
            schema: "public",
            table: "todo_sub_item");

        migrationBuilder.DropPrimaryKey(
            name: "pk_todo_sub_item",
            schema: "public",
            table: "todo_sub_item");

        migrationBuilder.RenameTable(
            name: "todo_sub_item",
            schema: "public",
            newName: "todo_sub_items",
            newSchema: "public");

        migrationBuilder.RenameIndex(
            name: "ix_todo_sub_item_todo_item_id",
            schema: "public",
            table: "todo_sub_items",
            newName: "ix_todo_sub_items_todo_item_id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_todo_sub_items",
            schema: "public",
            table: "todo_sub_items",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "fk_todo_sub_items_todo_items_todo_item_id",
            schema: "public",
            table: "todo_sub_items",
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
            name: "fk_todo_sub_items_todo_items_todo_item_id",
            schema: "public",
            table: "todo_sub_items");

        migrationBuilder.DropPrimaryKey(
            name: "pk_todo_sub_items",
            schema: "public",
            table: "todo_sub_items");

        migrationBuilder.RenameTable(
            name: "todo_sub_items",
            schema: "public",
            newName: "todo_sub_item",
            newSchema: "public");

        migrationBuilder.RenameIndex(
            name: "ix_todo_sub_items_todo_item_id",
            schema: "public",
            table: "todo_sub_item",
            newName: "ix_todo_sub_item_todo_item_id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_todo_sub_item",
            schema: "public",
            table: "todo_sub_item",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "fk_todo_sub_item_todo_items_todo_item_id",
            schema: "public",
            table: "todo_sub_item",
            column: "todo_item_id",
            principalSchema: "public",
            principalTable: "todo_items",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
