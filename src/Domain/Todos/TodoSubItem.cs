using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Todos;

public class TodoSubItem : Entity
{
    public Guid Id { get; set; }

    public Guid TodoItemId { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public DateTime? CompletedOn { get; set; }

    public int Order { get; set; }

    public TodoItem TodoItem { get; private set; } = null!;
}
