using Application.Common.Interfaces;
using Application.Elastic.Services;
using Domain.API;
using Domain.Users;
using SharedKernel.Abstractions.Messaging;

namespace Application.Dashboard;

internal sealed class GetDashboardQueryHandler(IUserContext userContext, IElasticDashboardService elasticService) : IQueryHandler<GetDashboardQuery, DashboardResponse>
{
    public async Task<Result<DashboardResponse>> Handle(GetDashboardQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId != userContext.UserId)
        {
            return Result.Failure<DashboardResponse>(UserErrors.Unauthorized());
        }

        DashboardResponse response = await elasticService.GetDashboardAsync(query.UserId, cancellationToken);

        return response;
    }
}
