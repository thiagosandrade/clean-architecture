using Application.Todos.Create;
using FluentValidation;

namespace Application.Todos.Breakdown;

public class TaskBreakdownCommandValidator : AbstractValidator<TaskBreakdownCommand>
{
    public TaskBreakdownCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.TodoId).NotEmpty();
    }
}
