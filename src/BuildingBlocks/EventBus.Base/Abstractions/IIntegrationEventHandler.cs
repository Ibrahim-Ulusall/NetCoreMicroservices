using EventBus.Base.Events;

namespace EventBus.Base.Abstractions;
public interface IIntegrationEventHandler<TIntegrationEventHandler> : IntegrationEventHandler where TIntegrationEventHandler : IntegrationEvent
{
    Task Handle(TIntegrationEventHandler @event);
}
