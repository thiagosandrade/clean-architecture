namespace Domain;

public sealed record PagedResponse<T>(IReadOnlyCollection<T> Items, int Total);
