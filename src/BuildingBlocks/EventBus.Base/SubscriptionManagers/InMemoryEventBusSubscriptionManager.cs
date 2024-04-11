using EventBus.Base.Abstractions;
using EventBus.Base.Events;

namespace EventBus.Base.SubscriptionManagers;
public class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionManager
{
    private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
    private readonly List<Type> _eventTypes;
    public Func<string, string> _eventNameGetter;

#pragma warning disable CS8618 
    public InMemoryEventBusSubscriptionManager(Func<string, string> eventNameGetter)
#pragma warning restore CS8618 
    {
        _handlers = new Dictionary<string, List<SubscriptionInfo>>();
        _eventTypes = new List<Type>();
        _eventNameGetter = eventNameGetter;
    }

    public bool IsEmpty => !_handlers.Keys.Any();

    public event EventHandler<string> OnEventRemoved;

    public void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        string eventName = GetEventKey<T>();
        AddSubscription(typeof(TH), eventName);
        if (!_eventTypes.Contains(typeof(T)))
        {
            _eventTypes.Add(typeof(T));
        }
    }
    private void AddSubscription(Type handlerType, string eventName)
    {
        if (!HasSubscriptionForEvent(eventName))
            _handlers.Add(eventName, new List<SubscriptionInfo>());
        if (_handlers[eventName].Any(s => s.HandleType == handlerType))
            throw new ArgumentException($"Handler Type : {handlerType.Name} '{eventName}' için zaten abone olunmuş", nameof(handlerType));
        _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));

    }


    public string GetEventKey<T>()
    {
        string name = typeof(T).Name;
        return _eventNameGetter(name);
    }

    public Type? GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(x => x.Name == eventName);

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
    {
        string key = GetEventKey<T>();
        return GetHandlersForEvent(key);
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];
    private SubscriptionInfo? FindSubscriptionToRemove<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        string key = GetEventKey<T>();
        return FindSubscriptionToRemove(key, typeof(TH));
    }
    private SubscriptionInfo? FindSubscriptionToRemove(string eventName, Type handlerType)
        => _handlers[eventName].SingleOrDefault(x => x.HandleType == handlerType);
    public void Clear() => _handlers.Clear();
    public bool HasSubscriptionForEvent<T>() where T : IntegrationEvent
    {
        string key = GetEventKey<T>();
        return HasSubscriptionForEvent(key);
    }

    public bool HasSubscriptionForEvent(string eventName) => _handlers.ContainsKey(eventName);

    public void RemoveSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        var handlerToRemove = FindSubscriptionToRemove<T, TH>();
        string eventNameToRemove = GetEventKey<T>();
        RemoveHandler(eventNameToRemove, handlerToRemove);
    }

    private void RemoveHandler(string eventName, SubscriptionInfo subToRemove)
    {
        if (subToRemove != null)
        {
            _handlers[eventName].Remove(subToRemove);
            if (!_handlers[eventName].Any())
            {
                _handlers.Remove(eventName);
                Type? eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                if (eventType != null)
                    _eventTypes.Remove(eventType);
                RaiseOnEventRemoved(eventName);
            }
        }
    }

    private void RaiseOnEventRemoved(string eventName)
    {
        var handler = OnEventRemoved;
        handler?.Invoke(this, eventName);
    }
}
