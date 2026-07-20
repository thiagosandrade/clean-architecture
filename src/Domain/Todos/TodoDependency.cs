using System;
using System.Collections.Generic;
using System.Text;
using SharedKernel;

namespace Domain.Todos;

public sealed class TodoDependency : Entity
{
    public Guid Id { get; set; }
    
    public Guid TodoItemId { get; private set; }

    public Guid DependsOnTodoItemId { get; private set; }

    public TodoItem TodoItem { get; private set; } = null!;

    public TodoItem DependsOnTodoItem { get; private set; } = null!;

    private TodoDependency()
    {
    }

    public TodoDependency(Guid todoItemId, Guid dependsOnTodoItemId)
    {
        TodoItemId = todoItemId;
        DependsOnTodoItemId = dependsOnTodoItemId;
    }
}
