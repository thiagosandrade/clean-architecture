using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddVectorIndex : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ix_todo_items_embedding
                ON todo_items
                USING hnsw (embedding vector_cosine_ops);
                ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS ix_todo_items_embedding;
                ");
    }
}
