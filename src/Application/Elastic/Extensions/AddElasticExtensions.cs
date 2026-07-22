using System;
using System.Collections.Generic;
using System.Text;
using Application.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Elastic.Extensions;

public static class AddElasticExtensions
{
    public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(_ =>
        {
            ElasticsearchClientSettings settings = new ElasticsearchClientSettings(
                new Uri(configuration["Elasticsearch:Uri"]!))
                .DefaultIndex("todos");

            return new ElasticsearchClient(settings);
        });

        services.AddScoped<IElasticTodoSearchService, ElasticTodoSearchService>();
        services.AddScoped<IElasticUserSearchService, ElasticUserSearchService>();
    }
}
