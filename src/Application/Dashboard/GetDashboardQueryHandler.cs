using Domain;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Data;
using SharedKernel.Abstractions.Messaging;
using SharedKernel.Authentication;

namespace Application.Dashboard;

internal sealed class GetDashboardQueryHandler(IApplicationDbContext context, IUserContext userContext)
    : IQueryHandler<GetDashboardQuery, DashboardResponse>
{
    public async Task<Result<DashboardResponse>> Handle(GetDashboardQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId != userContext.UserId)
        {
            return Result.Failure<DashboardResponse>(UserErrors.Unauthorized());
        }

        DateTime today = DateTime.UtcNow.Date;
        DateTime tomorrow = today.AddDays(1);

        IQueryable<TodoItem> baseQuery = context.TodoItems
            .AsNoTracking()
            .Where(t => t.UserId == query.UserId);

        // Summary counts
        int activeTasks = await baseQuery.Where(t => !t.IsCompleted).CountAsync(cancellationToken);
        int completedTasks = await baseQuery.Where(t => t.IsCompleted).CountAsync(cancellationToken);
        int dueToday = await baseQuery.Where(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value >= today && t.DueDate.Value < tomorrow).CountAsync(cancellationToken);
        int overdue = await baseQuery.Where(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value < today).CountAsync(cancellationToken);


        List<DashboardTaskResponse> recentlyUpdated = await GetRecentlyUpdated(baseQuery, cancellationToken);

        List<DashboardTaskResponse> overdueList = await GetOverdue(today, baseQuery, cancellationToken);

        List<DashboardTaskResponse> highPriority = await GetHighPriority(baseQuery, cancellationToken);

        List<DashboardTaskResponse> dueThisWeek = await GetDueThisWeek(today, baseQuery, cancellationToken);

        var response = new DashboardResponse
        {
            Summary = new DashboardSummaryResponse
            {
                ActiveTasks = activeTasks,
                CompletedTasks = completedTasks,
                DueToday = dueToday,
                Overdue = overdue
            },
            RecentlyUpdated = recentlyUpdated,
            Overdue = overdueList,
            HighPriority = highPriority,
            DueThisWeek = dueThisWeek
        };

        return response;
    }

    private static async Task<List<DashboardTaskResponse>> GetDueThisWeek(DateTime today, IQueryable<TodoItem> baseQuery, CancellationToken cancellationToken)
    {
        // DueThisWeek: Today <= DueDate <= Today+7 - Sort DueDate ASC - Take 5
        DateTime weekEnd = today.AddDays(7);

        List<DashboardTaskResponse> dueThisWeek = await baseQuery
            .Where(t => t.DueDate.HasValue && t.DueDate.Value >= today && t.DueDate.Value <= weekEnd && !t.IsCompleted)
            .OrderBy(t => t.DueDate)
            .Take(5)
            .Select(t => new DashboardTaskResponse
            {
                Id = t.Id,
                Description = t.Description,
                Priority = (int)t.Priority,
                DueDate = t.DueDate,
                IsCompleted = t.IsCompleted,
                UpdatedOn = t.UpdatedOn ?? t.CreatedOn
            })
            .ToListAsync(cancellationToken);
        return dueThisWeek;
    }

    private static async Task<List<DashboardTaskResponse>> GetHighPriority(IQueryable<TodoItem> baseQuery, CancellationToken cancellationToken)
    {
        // HighPriority: Priority == High AND IsCompleted == false - Sort DueDate ASC then UpdatedOn DESC - Take 5
        return await baseQuery
            .Where(t => t.Priority == Priority.High && !t.IsCompleted)
            .OrderBy(t => t.DueDate)
            .ThenByDescending(t => t.UpdatedOn ?? t.CreatedOn)
            .Take(5)
            .Select(t => new DashboardTaskResponse
            {
                Id = t.Id,
                Description = t.Description,
                Priority = (int)t.Priority,
                DueDate = t.DueDate,
                IsCompleted = t.IsCompleted,
                UpdatedOn = t.UpdatedOn ?? t.CreatedOn
            })
            .ToListAsync(cancellationToken);
    }

    private static async Task<List<DashboardTaskResponse>> GetOverdue(DateTime today, IQueryable<TodoItem> baseQuery, CancellationToken cancellationToken)
    {
        // Overdue: DueDate < Today AND IsCompleted == false - Sort DueDate ASC - Take 5
        return await baseQuery
            .Where(t => t.DueDate.HasValue && t.DueDate.Value < today && !t.IsCompleted)
            .OrderBy(t => t.DueDate)
            .Take(5)
            .Select(t => new DashboardTaskResponse
            {
                Id = t.Id,
                Description = t.Description,
                Priority = (int)t.Priority,
                DueDate = t.DueDate,
                IsCompleted = t.IsCompleted,
                UpdatedOn = t.UpdatedOn ?? t.CreatedOn
            })
            .ToListAsync(cancellationToken);
    }

    private static async Task<List<DashboardTaskResponse>> GetRecentlyUpdated(IQueryable<TodoItem> baseQuery, CancellationToken cancellationToken)
    {
        // RecentlyUpdated: UpdatedOn DESC - Take 5
        return await baseQuery
            .OrderByDescending(t => t.UpdatedOn ?? t.CreatedOn)
            .Take(5)
            .Select(t => new DashboardTaskResponse
            {
                Id = t.Id,
                Description = t.Description,
                Priority = (int)t.Priority,
                DueDate = t.DueDate,
                IsCompleted = t.IsCompleted,
                UpdatedOn = t.UpdatedOn ?? t.CreatedOn
            })
            .ToListAsync(cancellationToken);
    }
}
