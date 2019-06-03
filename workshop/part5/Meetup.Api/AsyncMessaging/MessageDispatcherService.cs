
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;

namespace Meetup.Api
{
    public class MessageDispatcherService
    {
        private readonly IEventStoreBus _bus;
        private readonly string _subscription;
        private readonly MessageHandlerRegistry _registry;
        private readonly MessageHandlerFactory _factory;
        private readonly ILogger<MessageDispatcherService> _logger;

        public MessageDispatcherService(MessageHandlerFactory factory,
            MessageHandlerRegistry registry,
            AsyncMessagingOptions options,
            IEventStoreBus bus,
            ILogger<MessageDispatcherService> logger)
        {
            _bus = bus;
            _subscription = options.Subscription;
            _registry = registry;
            _factory = factory;
            _logger = logger;
        }

        public void Start()
        {
            var serviceProvider = _factory.CreateBuilder();
            foreach (var subscription in _registry)
            {
                foreach (var handlerType in subscription.Value)
                {
                    _logger.LogDebug($"Subscribing {handlerType} to messsage {subscription.Key}");
                    Retry(() => Subscribe(subscription.Key, handlerType));
                }
            }

            void Subscribe(Type messageType, Type messageHandlerType)
            {
                _bus.Subscribe(messageType, _subscription,
                async (object msg) =>
                {
                    _logger.LogDebug($"Handling event type {messageType}");
                    dynamic handler = (IMessageHandler)serviceProvider.GetService(messageHandlerType);
                    await handler.Handle((dynamic)msg);
                });
            }

            void Retry(Action action, int retries = 5) =>
                Policy.Handle<Exception>()
                .WaitAndRetry(retries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .Execute(action);
        }
    }
}