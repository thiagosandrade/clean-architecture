using Application.Dashboard;
using Application.Elastic.Constants;
using Application.Elastic.Documents;
using Application.Elastic.Extensions;
using Domain.API;
using Domain.Todos;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;

namespace Application.Elastic.Services;

public interface IElasticDashboardService
{
    Task<DashboardResponse> GetDashboardAsync(Guid userId, CancellationToken cancellationToken = default);
}

internal sealed class ElasticDashboardService(ElasticsearchClient client, ILogger<ElasticDashboardService> logger) : IElasticDashboardService
{
    public async Task<DashboardResponse> GetDashboardAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        DateTime today = DateTime.UtcNow.Date;
        DateTime tomorrow = today.AddDays(1);
        DateTime weekEnd = today.AddDays(7);

        int activeTasks = await CountAsync(
        [
            UserFilter(userId),
            new TermQuery
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.IsCompleted)),
                Value = false
            }
        ],
        cancellationToken);

        int completedTasks = await CountAsync(
        [
            UserFilter(userId),
            new TermQuery
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.IsCompleted)),
                Value = true
            }
        ],
        cancellationToken);

        int dueToday = await CountAsync(
        [
            UserFilter(userId),
            new TermQuery
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.IsCompleted)),
                Value = false
            },
            new DateRangeQuery
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.DueDate)),
                Gte = today,
                Lt = tomorrow
            }
        ],
        cancellationToken);

        int overdueCount = await CountAsync(
        [
            UserFilter(userId),
            new TermQuery
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.IsCompleted)),
                Value = false
            },
            new DateRangeQuery
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.DueDate)),
                Lt = today
            }
        ],
        cancellationToken);

        List<DashboardTaskResponse> recentlyUpdated = await SearchAsync(
        [
            UserFilter(userId)
        ],
        [
            new FieldSort
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.UpdatedOn)),
                Order = SortOrder.Desc
            }
        ],
        cancellationToken);

        List<DashboardTaskResponse> overdue = await SearchAsync(
        [
            UserFilter(userId),
            new TermQuery
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.IsCompleted)),
                Value = false
            },
            new DateRangeQuery
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.DueDate)),
                Lt = today
            }
        ],
        [
            new FieldSort
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.DueDate)),
                Order = SortOrder.Asc
            }
        ],
        cancellationToken);

        List<DashboardTaskResponse> highPriority = await SearchAsync(
        [
            UserFilter(userId),
            new TermQuery
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.Priority)),
                Value = (int)Priority.High
            },
            new TermQuery
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.IsCompleted)),
                Value = false
            }
        ],
        [
            new FieldSort
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.DueDate)),
                Order = SortOrder.Asc
            },
            new FieldSort
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.UpdatedOn)),
                Order = SortOrder.Desc
            }
        ],
        cancellationToken);

        List<DashboardTaskResponse> dueThisWeek = await SearchAsync(
        [
            UserFilter(userId),
            new TermQuery
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.IsCompleted)),
                Value = false
            },
            new DateRangeQuery
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.DueDate)),
                Gte = today,
                Lte = weekEnd
            }
        ],
        [
            new FieldSort
            {
                Field = ElasticFieldName.FromProperty(nameof(TodoDocument.DueDate)),
                Order = SortOrder.Asc
            }
        ],
        cancellationToken);

        return new DashboardResponse
        {
            Summary = new DashboardSummaryResponse
            {
                ActiveTasks = activeTasks,
                CompletedTasks = completedTasks,
                DueToday = dueToday,
                Overdue = overdueCount
            },
            RecentlyUpdated = recentlyUpdated,
            Overdue = overdue,
            HighPriority = highPriority,
            DueThisWeek = dueThisWeek
        };
    }

    private async Task<int> CountAsync(List<Query> filters, CancellationToken cancellationToken)
    {
        SearchResponse<TodoDocument> response =
            await client.SearchAsync<TodoDocument>(
                new SearchRequest<TodoDocument>(
                    ElasticSearchConstants.ElasticTodoIndex)
                {
                    Query = new BoolQuery
                    {
                        Filter = filters
                    },

                    Size = 0,

                    TrackTotalHits = true
                },
                cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError("Elasticsearch count search failed: {Reason}", response.ElasticsearchServerError?.Error?.Reason);

            return 0;
        }

        return (int)response.Total;
    }

    private async Task<List<DashboardTaskResponse>> SearchAsync(List<Query> filters, ICollection<SortOptions> sort, CancellationToken cancellationToken)
    {
        SearchResponse<TodoDocument> response =
            await client.SearchAsync<TodoDocument>(
                new SearchRequest<TodoDocument>(
                    ElasticSearchConstants.ElasticTodoIndex)
                {
                    Query = new BoolQuery
                    {
                        Filter = filters
                    },

                    Size = 5,

                    Sort = sort
                },
                cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError("Elasticsearch dashboard search failed: {Reason}", response.ElasticsearchServerError?.Error?.Reason);

            return [];
        }

        return [.. response.Documents
            .Select(t => new DashboardTaskResponse
            {
                Id = t.Id,
                Description = t.Description,
                Priority = t.Priority,
                DueDate = t.DueDate,
                IsCompleted = t.IsCompleted,
                UpdatedOn = t.UpdatedOn ?? t.CreatedOn
            })];
    }

    private static TermQuery UserFilter(Guid userId) =>
        new()
        {
            Field = ElasticSearchConstants.UserId,
            Value = userId.ToString()
        };
}
