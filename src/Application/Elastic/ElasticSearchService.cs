using Domain.Todos;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedKernel.Abstractions.Data;

namespace Application.Elastic;

internal sealed class ElasticSearchService : IElasticSearchService
{
    private readonly ElasticsearchClient _client;
    private readonly IApplicationDbContext _context;

    public ElasticSearchService(ElasticsearchClient client, IApplicationDbContext context)
    {
        _client = client;
        _context = context;
    }

    public async Task IndexTodoAsync(
        Guid todoId,
        CancellationToken cancellationToken = default)
    {
        TodoItem? todo = await _context.TodoItems
            .AsNoTracking()
            .Include(t => t.SubItems)
            .Include(t => t.Attachments)
            .SingleOrDefaultAsync(
                t => t.Id == todoId,
                cancellationToken);

        if (todo is null)
        {
            return;
        }

        TodoSearchDocument document = new()
        {
            Id = todo.Id,
            UserId = todo.UserId,
            Description = todo.Description,
            Priority = (int)todo.Priority,
            DueDate = todo.DueDate,
            IsCompleted = todo.IsCompleted,
            CreatedOn = todo.CreatedOn,
            UpdatedOn = todo.UpdatedOn,

            Subtasks = [.. todo.SubItems
                .Select(s => new TodoSubtaskDocument
                {
                    Id = s.Id,
                    Description = s.Description,
                    IsCompleted = s.IsCompleted,
                    CompletedAt = s.CompletedAt,
                    Order = s.Order
                })],

            Attachments = [.. todo.Attachments
                .Select(a => new TodoAttachmentDocument
                {
                    Id = a.Id,
                    OriginalFileName = a.OriginalFileName,
                    ContentType = a.ContentType,
                    Size = a.Size
                })]
        };

        await _client.IndexAsync(
            document,
            idx => idx.Index("todos").Id(document.Id),
            cancellationToken);
    }
}

public sealed class TodoSearchDocument
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Description { get; init; } = string.Empty;
    public int Priority { get; init; }
    public DateTime? DueDate { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime? UpdatedOn { get; init; }

    public List<TodoSubtaskDocument> Subtasks { get; init; } = [];
    public List<TodoAttachmentDocument> Attachments { get; init; } = [];
}

public sealed class TodoSubtaskDocument
{
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public bool IsCompleted { get; init; }
    public DateTime? CompletedAt { get; init; }
    public int Order { get; init; }
}

public sealed class TodoAttachmentDocument
{
    public Guid Id { get; init; }
    public string OriginalFileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long Size { get; init; }
}
