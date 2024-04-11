using EventBus.AzureServiceBus;
using EventBus.Base;
using EventBus.Base.Abstractions;
using EventBus.Base.Enums;
using EventBus.RabbitMQ;

namespace EventBus.Factory;
public static class EventBusFactory
{
    public static IEventBus Create(EventBusConfig eventBusConfig, IServiceProvider serviceProvider)
    {
        return eventBusConfig.EventBusType switch
        {
            EventBusType.AzureSeviceBus => new EventBusServiceBus(serviceProvider, eventBusConfig),
            _ => new EventBusRabbitMQ(serviceProvider, eventBusConfig),
        };
    }
}
