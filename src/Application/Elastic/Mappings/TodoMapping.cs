using Application.Elastic.Constants;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;

namespace Application.Elastic.Mappings;

internal static class TodoIndexMappings
{
    public static CreateIndexRequest Create()
    {
        return new CreateIndexRequest(ElasticSearchConstants.ElasticTodoIndex)
        {
            Mappings = new TypeMapping
            {
                Properties = new Properties
                {
                    ["id"] = new KeywordProperty(),
                    ["userId"] = new KeywordProperty(),
                    ["description"] = new TextProperty(),
                    ["priority"] = new IntegerNumberProperty(),
                    ["priorityAsText"] = new KeywordProperty(),
                    ["isCompleted"] = new BooleanProperty(),

                    ["dueDate"] = new DateProperty(),
                    ["createdOn"] = new DateProperty(),
                    ["updatedOn"] = new DateProperty(),


                    ["labels"] = new KeywordProperty(),
                    ["categories"] = new KeywordProperty(),

                    ["embedding"] = new DenseVectorProperty
                    {
                        Dims = 1536,
                        Index = true,
                        Similarity = DenseVectorSimilarity.Cosine
                    },

                    ["subtasks"] = new NestedProperty
                    {
                        Properties = new Properties
                        {
                            ["id"] = new KeywordProperty(),
                            ["description"] = new TextProperty(),
                            ["isCompleted"] = new BooleanProperty(),
                            ["completedOn"] = new DateProperty(),
                            ["order"] = new IntegerNumberProperty()
                        }
                    },

                    ["attachments"] = new NestedProperty
                    {
                        Properties = new Properties
                        {
                            ["id"] = new KeywordProperty(),
                            ["originalFileName"] = new TextProperty(),
                            ["contentType"] = new KeywordProperty(),
                            ["size"] = new LongNumberProperty()
                        }
                    },

                    ["activities"] = new NestedProperty
                    {
                        Properties = new Properties
                        {
                            ["id"] = new KeywordProperty(),
                            ["activityType"] = new IntegerNumberProperty(),
                            ["activityTypeAsText"] = new KeywordProperty(),
                            ["description"] = new TextProperty(),
                            ["metadata"] = new TextProperty()
                        }
                    }
                }
            }
        };
    }
}
