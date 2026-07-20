namespace Domain;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }

    public List<IDomainEvent> DomainEvents => [.. _domainEvents];

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
