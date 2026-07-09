using Application.Todos.Breakdown;
using Application.Todos.Create;
using FluentValidation;

namespace Application.Todos.Rewrite;

public class SubtaskRewriteCommandValidator : AbstractValidator<SubtaskRewriteCommand>
{
    public SubtaskRewriteCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.TodoId).NotEmpty();
    }
}
