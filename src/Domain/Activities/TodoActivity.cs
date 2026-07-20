using System;
using System.Collections.Generic;
using System.Text;
using Domain.Todos;
using Domain.Users;

namespace Domain.Activities;

public sealed class TodoActivity : Entity
{
    public Guid Id { get; set; }

    public Guid TodoItemId { get; set; }

    public TodoItem TodoItem { get; set; } = default!;

    public TaskActivityType ActivityType { get; set; }

    public string Description { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public User User { get; set; }

    public string? Metadata { get; set; }
}
