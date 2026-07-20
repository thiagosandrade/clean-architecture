using Domain;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Data;
using SharedKernel.Abstractions.Messaging;

namespace Application.Users.GetAll;

internal sealed class GetAllQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetAllQuery, List<UserResponse>>
{
    public async Task<Result<List<UserResponse>>> Handle(GetAllQuery query, CancellationToken cancellationToken)
    {
        List<UserResponse> users = await context.Users
            .AsNoTracking()
            .Select(u => new UserResponse
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                CreatedOn = u.CreatedOn,
                Permissions = u.UserPermissions
                               .Select(p => new PermissionResponse { Id = p.Id, PermissionId = p.PermissionId, UserId = p.UserId, Description = p.Permission.Description})
                               .ToList()
            })
            .ToListAsync(cancellationToken);

        if (users is null)
        {
            return Result.Failure<List<UserResponse>>(UserErrors.NotFoundByEmail);
        }

        return users;
    }
}
