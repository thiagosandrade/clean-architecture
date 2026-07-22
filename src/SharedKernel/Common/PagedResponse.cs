namespace Domain.API;

public sealed record PagedResponse<T>(IReadOnlyCollection<T> Items, int Total);
