using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace EventBus.RabbitMQ;
public class EventBusRabbitMQ : BaseEventBus
{
    RabbitMQPersistentConnection _rabbitMQPersistentConnection;
    private readonly IConnectionFactory _connectionFactory = null!;
    private readonly IModel _consumerChannel;
    private ILogger _logger;
    public EventBusRabbitMQ(IServiceProvider serviceProvider, EventBusConfig eventBusConfig) : base(serviceProvider, eventBusConfig)
    {
        if (eventBusConfig.Connection != null)
        {
            var connectionJson = JsonConvert.SerializeObject(eventBusConfig.Connection, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            _connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connectionJson);
        }
        else
            _connectionFactory = new ConnectionFactory();
        _rabbitMQPersistentConnection = new RabbitMQPersistentConnection(_connectionFactory,
            EventBusConfig.ConnectionReTryCount, serviceProvider);
        _consumerChannel = CreateConsumerChannel();
        _logger = serviceProvider.GetService(typeof(ILogger<EventBusRabbitMQ>)) as ILogger<EventBusRabbitMQ>;
        _eventBusSubscriptionManager.OnEventRemoved += EventBusSubscriptionManagerOnEventRemoved;
    }

    private void EventBusSubscriptionManagerOnEventRemoved(object? sender, string eventName)
    {
        eventName = ProcessEventName(eventName);
        try
        {
            if (!_rabbitMQPersistentConnection.IsConnected) _rabbitMQPersistentConnection.TryConnect();
            _consumerChannel.QueueUnbind(queue: eventName, exchange: EventBusConfig.DefaultTopicName, routingKey: eventName);
            if (_eventBusSubscriptionManager.IsEmpty)
                _consumerChannel.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError("EventBusRabbitMQ : Abonelik iptal edilirken bir hata meydana geldi. Hata : {messge}", ex.Message);
        }
    }

    public override void Publish(IntegrationEvent @event)
    {
        if (_rabbitMQPersistentConnection.IsConnected) _rabbitMQPersistentConnection.TryConnect();
        string eventName = @event.GetType().Name;
        eventName = ProcessEventName(eventName);
        CreateConsumerChannel();
        string message = JsonConvert.SerializeObject(@event);
        byte[] body = Encoding.UTF8.GetBytes(message);

        IBasicProperties properties = _consumerChannel.CreateBasicProperties();
        properties.DeliveryMode = 2;//persistent

        _consumerChannel.QueueDeclare(queue: GetSubName(eventName), durable: true,
            exclusive: false, autoDelete: false, arguments: null);
        
        _consumerChannel.BasicPublish(exchange: EventBusConfig.DefaultTopicName,
            routingKey: eventName, mandatory: true,
            basicProperties: properties, body: body);
    }

    public override void Subscribe<T, TH>()
    {
        string eventName = typeof(T).Name;
        eventName = ProcessEventName(eventName);
        if (!_eventBusSubscriptionManager.HasSubscriptionForEvent(eventName))
        {
            if (!_rabbitMQPersistentConnection.IsConnected) _rabbitMQPersistentConnection.TryConnect();
            _consumerChannel.QueueDeclare(queue: GetSubName(eventName),
                durable: true, exclusive: false, autoDelete: false, arguments: null);

            _consumerChannel.QueueBind(queue: GetSubName(eventName),
                exchange: EventBusConfig.DefaultTopicName, routingKey: eventName);
        }
        _eventBusSubscriptionManager.AddSubscription<T, TH>();
        StartBasicConsume(eventName);
    }

    public override void UnSubscribe<T, TH>()
    {
        _eventBusSubscriptionManager.RemoveSubscription<T, TH>();

    }

    private IModel CreateConsumerChannel(string exchangeType = "direct")
    {
        if (!_rabbitMQPersistentConnection.IsConnected) _rabbitMQPersistentConnection.TryConnect();
        IModel channel = _rabbitMQPersistentConnection.CreateModel();
        channel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: exchangeType);
        return channel;
    }

    private void StartBasicConsume(string eventName)
    {
        if (_consumerChannel != null)
        {
            var consumer = new EventingBasicConsumer(_consumerChannel);
            consumer.Received += ConsumerReceived;
            _consumerChannel.BasicConsume(queue: GetSubName(eventName), autoAck: false, consumer: consumer);
        }
    }

    private async void ConsumerReceived(object? sender, BasicDeliverEventArgs e)
    {
        string eventName = e.RoutingKey;
        eventName = ProcessEventName(eventName);
        string message = Encoding.UTF8.GetString(e.Body.Span);
        try
        {
            await ProcessEvent(eventName, message);
        }
        catch (Exception ex)
        {
            _logger.LogError("EventBusRabbitMQ Mesasge: Mesaj Consume edilirken hata meydana geldi. {message}", ex.Message);
        }
        _consumerChannel.BasicAck(e.DeliveryTag, multiple: false);
    }
}
