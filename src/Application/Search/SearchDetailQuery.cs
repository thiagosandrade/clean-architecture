using SharedKernel.Abstractions.Messaging;

namespace Application.Search;

public sealed record SearchDetailQuery(Guid UserId, string Type, Guid Id) : IQuery<SearchDetailResponse>;
