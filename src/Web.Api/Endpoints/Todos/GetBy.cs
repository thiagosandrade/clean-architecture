using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.Todos.GetBy;
using Application.Todos.GetById;
using Domain.Todos;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

public sealed class GetSearchByRequest
{
    public Guid UserId { get; set; }
    public string Description { get; set; }
}

internal sealed class GetBy : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("todos/searchby", async (
            [AsParameters] GetSearchByRequest request,
            IQueryHandler<GetTaskQuery, List<GetTaskQueryResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new GetTaskQuery(request.UserId, request.Description);

            Result<List<GetTaskQueryResponse>> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .HasPermission(PermissionsConstants.TodoAccess);
    }
}
