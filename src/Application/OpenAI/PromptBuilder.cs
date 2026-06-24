namespace Application.OpenAI;

public static class PromptBuilder
{
    public static string TodoCategories(string description)
    {
        return $"""
            Categorize the following description into a list of relevant categories. 
            Return the categories as a JSON object with a single property "Categories" which is an array of strings.
            Description: "{description}"
            """;
    }
    public static string TodoDescription(string description, IEnumerable<string> categories)
    {
        string categoriesList = string.Join(", ", categories);

        return $"""
            Generate a concise description based on the following text and its associated categories. 
            Description: "{description}"
            Categories: {categoriesList}
            """;
    }

    public static string ParseTextForTodo(string text, Guid UserId) 
    {
        return $"""
            Parse the following text and extract the relevant information to create a todo item. 
            Return the extracted information as a JSON object with the following properties:
            - Description: string
            - DueDate: string (in ISO 8601 format, or null if not present)
            - Labels: array of strings or empty array if not present
            - Priority: string (one of "Low", "Medium", "High", or empty array if not present)
            - UserId: {UserId}
            Text: "{text}"
            """;
    }
}
