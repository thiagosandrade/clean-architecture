using Application.Abstractions.Messaging;
using Application.Search;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using SharedKernel;

namespace Web.Api.Endpoints.Search;

public sealed class SearchDetailRequest
{
    public Guid UserId { get; init; }
}

internal sealed class Details : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("search/details/{type}/{id:Guid}", async (
            string type,
            Guid id,
            [AsParameters] SearchDetailRequest request,
            IQueryHandler<SearchDetailQuery, SearchDetailResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new SearchDetailQuery(request.UserId, type, id);

            Result<SearchDetailResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Search);
    }
}
