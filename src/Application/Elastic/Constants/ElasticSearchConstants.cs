namespace Application.Elastic.Constants;

public static class ElasticSearchConstants
{
    internal const int BatchSize = 1000;

    internal const string ElasticUserIndex = "users";
    internal const string ElasticTodoIndex = "todos";
    

    internal static readonly string[] UserFields = ["firstName", "lastName", "email"];
    internal static readonly string[] TodoFields = ["description", "subtasks.description", "attachments.originalFileName"];

    internal const string UserId = "userId";
    internal const string UserIdKeyword = "userId.keyword";
    internal const string Id = "id";
    internal const string IdKeyword = "id.keyword";
}
