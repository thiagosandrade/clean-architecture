using Application.Abstractions.Messaging;
using SharedKernel;
using System;
using System.Collections.Generic;

namespace Application.Dashboard;

public sealed class GetDashboardQuery : IQuery<DashboardResponse>
{
    public GetDashboardQuery(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}

public sealed class DashboardResponse
{
    public DashboardSummaryResponse Summary { get; set; }

    public IReadOnlyList<DashboardTaskResponse> RecentlyUpdated { get; set; } = Array.Empty<DashboardTaskResponse>();

    public IReadOnlyList<DashboardTaskResponse> Overdue { get; set; } = Array.Empty<DashboardTaskResponse>();

    public IReadOnlyList<DashboardTaskResponse> HighPriority { get; set; } = Array.Empty<DashboardTaskResponse>();

    public IReadOnlyList<DashboardTaskResponse> DueThisWeek { get; set; } = Array.Empty<DashboardTaskResponse>();
}

public sealed class DashboardSummaryResponse
{
    public int ActiveTasks { get; set; }

    public int CompletedTasks { get; set; }

    public int DueToday { get; set; }

    public int Overdue { get; set; }
}

public sealed class DashboardTaskResponse
{
    public Guid Id { get; set; }

    public string Description { get; set; }

    public int Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime UpdatedOn { get; set; }
}
