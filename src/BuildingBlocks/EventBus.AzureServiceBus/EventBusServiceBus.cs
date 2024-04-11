using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace EventBus.AzureServiceBus;
public class EventBusServiceBus : BaseEventBus
{
    ITopicClient _topicClient;
    ManagementClient _managementClient;
    private ILogger _logger = null!;
    public EventBusServiceBus(IServiceProvider serviceProvider, EventBusConfig eventBusConfig) : base(serviceProvider, eventBusConfig)
    {
        _managementClient = new ManagementClient(eventBusConfig.EventBusConnectionString);
        _topicClient = CreateTopicClient();
        _logger = serviceProvider.GetService(typeof(ILogger<EventBusServiceBus>)) as ILogger<EventBusServiceBus>;
    }


    private ITopicClient CreateTopicClient()
    {
        if (_topicClient == null || _topicClient.IsClosedOrClosing)
        {
            _topicClient = new TopicClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName, RetryPolicy.Default);
        }

        if (!_managementClient.TopicExistsAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult())
        {
            _managementClient.CreateTopicAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult();
        }
        return _topicClient;
    }
    public override void Publish(IntegrationEvent @event)
    {
        string eventName = @event.GetType().Name;
        eventName = ProcessEventName(eventName);

        var message = new Message
        {
            MessageId = Guid.NewGuid().ToString(),
            Label = eventName,
            Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event))
        };
        _topicClient.SendAsync(message).GetAwaiter().GetResult();
    }

    public override void Subscribe<T, TH>()
    {
        string eventName = typeof(T).Name;
        eventName = ProcessEventName(eventName);
        if (_eventBusSubscriptionManager.HasSubscriptionForEvent(eventName))
        {
            ISubscriptionClient subscriptionClient = CreateSubscriptionClientIfNotExists(eventName);
            RegisterSubscriptionClientMessageHandler(subscriptionClient);
        }
        _eventBusSubscriptionManager.AddSubscription<T, TH>();
        _logger.LogInformation("{EventName} {EventHandler} abone oldu", eventName, typeof(TH).Name);
    }

    public override void UnSubscribe<T, TH>()
    {

        string eventName = typeof(T).Name;
        try
        {
            ISubscriptionClient subscriptionClient = CreateSubscriptionClient(eventName);
            subscriptionClient.RemoveRuleAsync(eventName).GetAwaiter().GetResult();
        }
        catch (MessagingEntityNotFoundException)
        {
            _logger.LogWarning("{EventName} bulunamadı", eventName);
        }
        _logger.LogInformation("{EventName} aboneliği silindi", eventName);
        _eventBusSubscriptionManager.RemoveSubscription<T, TH>();
    }

    private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subscriptionClient)
    {
        subscriptionClient.RegisterMessageHandler(async (message, cancellationToken) =>
        {
            string eventName = message.Label;
            string data = Encoding.UTF8.GetString(message.Body);
            if (await ProcessEvent(ProcessEventName(eventName), data))
            {
                await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            }
        }, new MessageHandlerOptions(ExceptionReceviedHandler) { MaxConcurrentCalls = 10, AutoComplete = false });
    }

    private Task ExceptionReceviedHandler(ExceptionReceivedEventArgs args)
    {
        var exception = args.Exception;
        var context = args.ExceptionReceivedContext;
        _logger.LogWarning(exception, "Error Handling Message : {ExceptionMessage} - Context: {@ExceptionContext}", exception.Message, context);
        return Task.CompletedTask;
    }

    private ISubscriptionClient CreateSubscriptionClientIfNotExists(string eventName)
    {
        SubscriptionClient subClient = CreateSubscriptionClient(eventName);
        bool exists = _managementClient.SubscriptionExistsAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();
        if (!exists)
        {
            _managementClient.CreateSubscriptionAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();
            RemoveDefaultRule(subClient);
        }
        CreateRuleIfNotExists(ProcessEventName(eventName), subClient);
        return subClient;
    }

    private void CreateRuleIfNotExists(string eventName, ISubscriptionClient subscriptionClient)
    {
        bool ruleExists;
        try
        {
            var rule = _managementClient.GetRuleAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName), eventName).GetAwaiter().GetResult();
            ruleExists = rule != null;

        }
        catch (MessagingEntityNotFoundException)
        {
            ruleExists = false;
        }
        if (!ruleExists)
        {
            subscriptionClient.AddRuleAsync(new RuleDescription()
            {
                Name = eventName,
                Filter = new CorrelationFilter() { Label = eventName }
            }).GetAwaiter().GetResult();
        }
    }
    private void RemoveDefaultRule(SubscriptionClient subscriptionClient)
    {
        try
        {
            subscriptionClient.RemoveRuleAsync(RuleDescription.DefaultRuleName).GetAwaiter().GetResult();
        }
        catch (MessagingEntityNotFoundException)
        {
            _logger.LogWarning("{DefaultRuleName}", RuleDescription.DefaultRuleName);
        }
    }

    private SubscriptionClient CreateSubscriptionClient(string eventName)
     => new SubscriptionClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName, GetSubName(eventName));

    public override void Dispose()
    {
        base.Dispose();
        _topicClient.CloseAsync().GetAwaiter().GetResult();
        _managementClient.CloseAsync().GetAwaiter().GetResult();
        _topicClient = null;
        _managementClient = null;
    }
}
