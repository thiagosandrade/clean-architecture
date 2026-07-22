using Application.RabbitMq.Configuration;

namespace Application.RabbitMq.Events;

[RabbitMqEvent(exchange: "users", routingKey: "user.created")]
public sealed record UserCreatedIntegrationEvent(Guid UserId) : IIntegrationEvent;

[RabbitMqEvent(exchange: "users", routingKey: "user.updated")]
public sealed record UserUpdatedIntegrationEvent(Guid UserId) : IIntegrationEvent;

[RabbitMqEvent(exchange: "users", routingKey: "user.deleted")]
public sealed record UserDeletedIntegrationEvent(Guid UserId) : IIntegrationEvent;






