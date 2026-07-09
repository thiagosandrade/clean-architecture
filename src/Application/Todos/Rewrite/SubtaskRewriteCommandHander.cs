using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.OpenAI.Enrichment;
using Application.Todos.Breakdown;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.Rewrite;

internal sealed class SubtaskRewriteCommandHander(
    IApplicationDbContext context,
    IRewriteEnrichmentService subtaskRewriteService,
    IUserContext userContext)
    : ICommandHandler<SubtaskRewriteCommand, SubtaskRewriteResponse>
{
    public async Task<Result<SubtaskRewriteResponse>> Handle(SubtaskRewriteCommand command, CancellationToken cancellationToken)
    {
        if (userContext.UserId != command.UserId)
        {
            return Result.Failure<SubtaskRewriteResponse>(UserErrors.Unauthorized());
        }

        User? user = await context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<SubtaskRewriteResponse>(UserErrors.NotFound(command.UserId));
        }

        SubtaskRewriteResponse response = await subtaskRewriteService.RewriteAsync(command.Description, command.Style, cancellationToken);

        return Result.Success<SubtaskRewriteResponse>(response);
    }
}
