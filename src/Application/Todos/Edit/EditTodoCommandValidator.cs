using FluentValidation;

namespace Application.Todos.Edit;

public class EditTodoCommandValidator : AbstractValidator<EditTodoCommand>
{
    public EditTodoCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.Priority).IsInEnum();
        RuleFor(c => c.Description).NotEmpty().MaximumLength(255);
        RuleFor(c => c.DueDate).GreaterThanOrEqualTo(DateTime.MinValue).When(x => x.DueDate.HasValue);
    }
}
