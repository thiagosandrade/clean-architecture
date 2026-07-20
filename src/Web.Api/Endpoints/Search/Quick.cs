using Application.Search;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Extensions;
using SharedKernel.Abstractions.Messaging;
using Domain;

namespace Web.Api.Endpoints.Search;

public sealed class QuickSearchRequest
{
    public Guid UserId { get; init; }
    public string Text { get; init; }
    public int Limit { get; init; } = 5;
}

internal sealed class Quick : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("search/quick", async (
            [FromBody] QuickSearchRequest request,
            IQueryHandler<QuickSearchQuery, QuickSearchResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new QuickSearchQuery(request.UserId, request.Text, request.Limit);

            Result<QuickSearchResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Search);
    }
}
