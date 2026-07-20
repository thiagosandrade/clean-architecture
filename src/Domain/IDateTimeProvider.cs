namespace Domain;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
