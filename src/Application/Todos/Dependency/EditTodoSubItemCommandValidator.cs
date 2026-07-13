using FluentValidation;

namespace Application.Todos.Dependency;

public class EditTodoDependenciesCommandValidator : AbstractValidator<EditTodoDependenciesCommand>
{
    public EditTodoDependenciesCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleForEach(c => c.Dependencies).ChildRules(subItem =>
        {
            subItem.RuleFor(x => x).NotEmpty();
        });
    }
}
