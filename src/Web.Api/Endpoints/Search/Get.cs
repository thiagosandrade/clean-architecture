using Application.Abstractions.Messaging;
using Application.Search;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using SharedKernel;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Endpoints.Search;

public sealed class SearchRequest
{
    public Guid UserId { get; init; }
    public string Text { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("search", async (
            [FromBody] SearchRequest request,
            IQueryHandler<SearchQuery, SearchResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new SearchQuery(request.UserId, request.Text, request.Page, request.PageSize);

            Result<SearchResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Search);
    }
}
