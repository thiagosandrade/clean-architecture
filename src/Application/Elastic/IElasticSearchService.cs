using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Elastic;

public interface IElasticSearchService
{
    /// <summary>
    /// Indexes a todo item with its related entities into Elasticsearch.
    /// </summary>
    Task IndexTodoAsync(Guid todoId, CancellationToken cancellationToken = default);
}
