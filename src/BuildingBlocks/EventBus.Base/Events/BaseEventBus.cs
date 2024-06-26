﻿using EventBus.Base.Abstractions;
using EventBus.Base.SubscriptionManagers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EventBus.Base.Events;
public abstract class BaseEventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    public readonly IEventBusSubscriptionManager _eventBusSubscriptionManager;
    public EventBusConfig EventBusConfig { get; set; } 

    protected BaseEventBus(IServiceProvider serviceProvider, EventBusConfig eventBusConfig)
    {
        _serviceProvider = serviceProvider;
        EventBusConfig = eventBusConfig;
        _eventBusSubscriptionManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
    }

    public virtual string ProcessEventName(string eventName)
    {
        if (EventBusConfig.DeleteEventPrefix)
            eventName = eventName.TrimStart(EventBusConfig.EventNamePrefix.ToArray());
        if (EventBusConfig.DeleteEventSuffix)
            eventName = eventName.TrimEnd(EventBusConfig.EventNameSuffix.ToArray());
        return eventName;
    }

    public virtual string GetSubName(string eventName) => $"{EventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
    public virtual void Dispose() => EventBusConfig = null;

    public async Task<bool> ProcessEvent(string eventName, string message)
    {
        eventName = ProcessEventName(eventName);
        bool processed = false;

        if (_eventBusSubscriptionManager.HasSubscriptionForEvent(eventName))
        {
            IEnumerable<SubscriptionInfo> subscriptions = _eventBusSubscriptionManager.GetHandlersForEvent(eventName);
            using (var scope = _serviceProvider.CreateScope())
            {
                foreach (SubscriptionInfo subscription in subscriptions)
                {
                    var handler = _serviceProvider.GetService(subscription.HandleType);
                    if (handler == null) continue;
                    var eventType = _eventBusSubscriptionManager.GetEventTypeByName($"{EventBusConfig.EventNamePrefix}{eventName}{EventBusConfig.EventNameSuffix}");
                    var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                }
            }
            processed = true;
        }
        return processed;
    }
    public abstract void Publish(IntegrationEvent @event);
    public abstract void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    public abstract void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
}
