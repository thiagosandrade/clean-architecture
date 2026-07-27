using Application.Elastic.Constants;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;

namespace Application.Elastic.Mappings;

internal static class UserIndexMappings
{

    public static CreateIndexRequest Create()
    {
        return new CreateIndexRequest(ElasticSearchConstants.ElasticUserIndex)
        {
            Mappings = new TypeMapping
            {
                Properties = new Properties
                {
                    ["id"] = new KeywordProperty(),

                    ["firstName"] = new TextProperty(),

                    ["lastName"] = new TextProperty(),

                    ["email"] = new TextProperty()
                }
            }
        };
    }
}
