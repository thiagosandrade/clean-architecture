namespace Application.OpenAI.Parser;

public class TodoExtractorResponse
{
    public string? Description { get; set; }

    public List<string>? Categories { get; set; }

    public List<string>? Labels { get; set; }

    public string? Priority { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public Guid UserId { get; set; }
}
