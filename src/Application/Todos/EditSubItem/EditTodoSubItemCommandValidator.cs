using FluentValidation;

namespace Application.Todos.EditSubItem;

public class EditTodoSubItemCommandValidator : AbstractValidator<EditTodoSubItemsCommand>
{
    public EditTodoSubItemCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleForEach(c => c.TodoSubItems).ChildRules(subItem =>
        {
            subItem.RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            subItem.RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        });
    }
}
