using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Application.Abstractions.Data;
using Domain.Activities;
using Microsoft.EntityFrameworkCore;

namespace Application.Todos.Activities.Log;

public class TodoActivityService(IApplicationDbContext applicationDbContext) : ITodoActivityService
{
    public async Task LogAsync(
        Guid todoId,
        TaskActivityType activityType,
        string description,
        Guid userId,
        object? metadata = null,
        CancellationToken cancellationToken = default)
    {
        var activity = new TodoActivity
        {
            Id = Guid.NewGuid(),

            TodoItemId = todoId,

            ActivityType = activityType,

            Description = description,

            UserId = userId,

            CreatedAtUtc = DateTime.UtcNow,

            Metadata = metadata == null
                ? null
                : JsonSerializer.Serialize(metadata)
        };

        applicationDbContext.TodoActivities.Add(activity);

        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}
