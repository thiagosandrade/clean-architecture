using Application.RabbitMq.Configuration;

namespace Application.RabbitMq.Events;

[RabbitMqEvent(exchange: "todos", routingKey: "todo.created")]
public sealed record TodoCreatedIntegrationEvent(Guid TodoId, Guid UserId, string Description) : IIntegrationEvent;

[RabbitMqEvent(exchange: "todos", routingKey: "todo.updated")]
public sealed record TodoUpdatedIntegrationEvent(Guid TodoId, Guid UserId, string Description) : IIntegrationEvent;

[RabbitMqEvent(exchange: "todos", routingKey: "todo.deleted")]
public sealed record TodoDeletedIntegrationEvent(Guid TodoId, Guid UserId, string Description) : IIntegrationEvent;



[RabbitMqEvent(exchange: "todos", routingKey: "todo.attachment")]
public sealed record TodoAttachmentIntegrationEvent(Guid TodoId, Guid UserId, string Description) : IIntegrationEvent;

[RabbitMqEvent(exchange: "todos", routingKey: "todo.breakdown")]
public sealed record TodoBreakdownIntegrationEvent(Guid TodoId, Guid UserId, string Description) : IIntegrationEvent;

[RabbitMqEvent(exchange: "todos", routingKey: "todo.dependency")]
public sealed record TodoDependencyIntegrationEvent(Guid TodoId, Guid UserId, string Description) : IIntegrationEvent;

[RabbitMqEvent(exchange: "todos", routingKey: "todo.rewrite")]
public sealed record TodoRewriteIntegrationEvent(Guid TodoId, Guid UserId, string Description) : IIntegrationEvent;


