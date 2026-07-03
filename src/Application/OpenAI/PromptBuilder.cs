namespace Application.OpenAI;

public static class PromptBuilder
{
    public static string TodoCategories(string description) =>
        $"""
            Categorize the following description into a list of relevant categories. 
            Return the categories as a JSON object with a single property "Categories" which is an array of strings.
            Description: "{description}"
            """;
    
    public static string TodoDescription(string description, IEnumerable<string> labels, IEnumerable<string> categories)
    {
        // This is used for embeddings only,  
        // it does not need to be a valid JSON,
        // but it should be a structured text that includes the description, labels, and categories.

        string labelsList = string.Join(", ", labels);
        string categoriesList = string.Join(", ", categories);

        return $"""
            Description: "{description}"
            Labels: {labelsList}
            Categories: {categoriesList}
            """;
    }

    public static string ParseTextForTodo(string text, Guid UserId) =>
        $"""
            Parse the following text and extract the relevant information to create a todo item. 
            Return the extracted information as a JSON object with the following properties:
            - Description: string
            - DueDate: string (in ISO 8601 format, or null if not present)
            - Labels: array of strings or empty array if not present
            - Priority: string (one of "Low", "Medium", "High", or empty array if not present)
            - UserId: {UserId}
            Text: "{text}"
            """;

    public static string SubTaskBreakdown(string description) =>
        $$"""
        TODO:
        {{description}}

        Output:

        {
          "subTasks": [
            "subtask 1",
            "subtask 2"
          ]
        }
        """;
}
