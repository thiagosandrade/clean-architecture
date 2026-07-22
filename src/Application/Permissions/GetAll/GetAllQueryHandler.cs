using Application.Common.Interfaces;
using Domain.API;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Messaging;

namespace Application.Permissions.GetAll;

internal sealed class GetAllQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetAllQuery, List<PermissionResponse>>
{
    public async Task<Result<List<PermissionResponse>>> Handle(GetAllQuery query, CancellationToken cancellationToken)
    {
        List<PermissionResponse> users = await context.Permissions
            .Select(u => new PermissionResponse
            {
                Id = u.Id,
                Description = u.Description
            })
            .ToListAsync(cancellationToken);

        return users;
    }
}
