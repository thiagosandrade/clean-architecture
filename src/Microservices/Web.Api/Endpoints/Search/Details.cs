using Application.Search;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Abstractions.Messaging;
using Domain.API;
using Infrastructure.Extensions;

namespace Web.Api.Endpoints.Search;

public sealed class SearchDetailRequest
{
    public Guid UserId { get; init; }
}

internal sealed class Details : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("search/details/{type}/{id:Guid}", async (
            string type,
            Guid id,
            [FromBody] SearchDetailRequest request,
            IQueryHandler<GetSearchDetailQuery, SearchDetailResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSearchDetailQuery(request.UserId, type, id);

            Result<SearchDetailResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Search);
    }
}
