using System;
using System.Collections.Generic;
using System.Text;
using Application.RabbitMq.Events;

namespace Application.RabbitMq.Configuration;

public interface IIntegrationEventHandler<in TIntegrationEvent> 
    where TIntegrationEvent : IIntegrationEvent
{
    Task Handle(TIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}
