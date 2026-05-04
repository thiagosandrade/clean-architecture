using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

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
