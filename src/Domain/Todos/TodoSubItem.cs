using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using SharedKernel;

namespace Domain.Todos;

public class TodoSubItem : Entity
{
    public Guid Id { get; set; }

    public Guid TodoItemId { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int Order { get; set; }
}
