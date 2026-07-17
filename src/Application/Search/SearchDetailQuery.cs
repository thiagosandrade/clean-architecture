using Application.Abstractions.Messaging;
using SharedKernel;
using System;

namespace Application.Search;

public sealed record SearchDetailQuery(Guid UserId, string Type, Guid Id) : IQuery<SearchDetailResponse>;
