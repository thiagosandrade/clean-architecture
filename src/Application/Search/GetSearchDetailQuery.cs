using SharedKernel.Abstractions.Messaging;

namespace Application.Search;

public sealed record GetSearchDetailQuery(Guid UserId, string Type, Guid Id) : IQuery<SearchDetailResponse>;
