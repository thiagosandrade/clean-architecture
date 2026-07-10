using Application.Todos.GetById;
using Domain.Activities;
using Domain.Todos;

namespace Application.TaskActivities.Get;

public class GetTaskActivityResponse
{
    public GetTaskActivityResponse(List<TaskActivity> resultActivities)
    {
        Activities = resultActivities.Select(x => new ActivityResponse()
        {
            Id = x.Id,
            ActivityType = x.ActivityType,
            Description = x.Description,
            Username = $"{x.User.FirstName} {x.User.LastName}",
            CreatedAtUtc = x.CreatedAtUtc,
        });
    }

    public IEnumerable<ActivityResponse> Activities { get; }
}

public class ActivityResponse
{
    public Guid Id { get; set; }

    public TaskActivityType ActivityType { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}
