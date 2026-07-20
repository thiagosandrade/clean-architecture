using System;
using System.Collections.Generic;

namespace Application.Search;

public sealed class SearchDetailResponse
{
    public string Type { get; init; } = string.Empty;

    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Subtitle { get; init; } = string.Empty;


    public SearchDetailSummary Summary { get; init; } = new();


    public List<SearchDetailLink> Links { get; init; } = [];

    public object? Data { get; init; }
}

public sealed class SearchDetailSummary
{
    public string CreatedBy { get; init; } = string.Empty;

    public DateTime? CreatedOn { get; init; }

    public DateTime? UpdatedOn { get; init; }


    public string Status { get; init; } = string.Empty;
}

public sealed class SearchDetailLink
{
    public string Type { get; init; } = string.Empty;

    public Guid Id { get; init; }

    public string Description { get; init; } = string.Empty;
}
