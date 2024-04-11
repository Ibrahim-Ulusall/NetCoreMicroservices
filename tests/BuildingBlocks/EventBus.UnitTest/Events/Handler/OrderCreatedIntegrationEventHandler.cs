using EventBus.Base.Abstractions;
using EventBus.UnitTest.Events.Event;

namespace EventBus.UnitTest.Events.Handler;
public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    public Task Handle(OrderCreatedIntegrationEvent @event)
    {
        return Task.CompletedTask;
    }
}
