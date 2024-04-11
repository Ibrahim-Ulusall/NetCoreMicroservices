using EventBus.Base;
using EventBus.Base.Abstractions;
using EventBus.Factory;
using EventBus.UnitTest.Events.Event;
using EventBus.UnitTest.Events.Handler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventBus.UnitTest
{
    [TestClass]
    public class EventBusTests
    {

        private ServiceCollection services;

        public EventBusTests()
        {
            services = new ServiceCollection();
            services.AddLogging(cfg => cfg.AddConsole());
        }

        [TestMethod]
        public void SubscribeEventOnRabbitMQTest()
        {

            services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetRabbitMqConfiguration(), sp);
            });
            var serviceProvider = services.BuildServiceProvider();
            var eventBus = serviceProvider.GetService<IEventBus>();
            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

        }
        [TestMethod]
        public void SendMessageRabbitMQTest()
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetRabbitMqConfiguration(), sp);
            });
            var serviceProvider = services.BuildServiceProvider();
            var eventBus = serviceProvider.GetService<IEventBus>();
            eventBus.Publish(new OrderCreatedIntegrationEvent(1));
        }
        private EventBusConfig GetRabbitMqConfiguration()
        {
            return new EventBusConfig
            {
                ConnectionReTryCount = 5,
                EventBusType = Base.Enums.EventBusType.RabbitMQ,
                DefaultTopicName = "NetCoreMicroservices.Test",
                SubscriberClientAppName = "EventBus.UnitTest",
                EventNameSuffix = "IntegrationEvent"
            };
        }
    }
}