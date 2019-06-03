using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Meetup.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Meetup.Api
{
    internal class EventStorePersistentSubscription : IEventStoreBus
    {
        private readonly IEventStoreConnection _conn;
        private readonly EventDeserializer _eventDeserializer;
        private readonly UserCredentials _credentials;

        private readonly ILogger<EventStorePersistentSubscription> _logger;

        public EventStorePersistentSubscription(IEventStoreConnection conn, IConfiguration config, EventDeserializer eventDeserializer, ILogger<EventStorePersistentSubscription> logger)
        {
            _conn = conn;
            _eventDeserializer = eventDeserializer;
            _credentials = new UserCredentials(config["subscription_user"] ?? "admin", config["subscriptions_password"] ?? "changeit");
            _logger = logger;
        }

        public async Task Subscribe(Type eventType, string subscription, Func<object, Task> eventHandler)
        {
            var eventTypeStream = $"$et-{eventType.Name}";
            var settings = PersistentSubscriptionSettings.Create().ResolveLinkTos().StartFromCurrent();

            _logger.LogDebug($"Subscribing to {eventTypeStream} with {_credentials.Username}:{_credentials.Password}");

            await CreateSubscription(subscription, eventTypeStream, settings);

            await _conn.ConnectToPersistentSubscriptionAsync(
                eventTypeStream,
                subscription,
                (_, ev) =>
                {
                    _logger.LogDebug($"Event Apperead {ev.Event}");
                    eventHandler(_eventDeserializer.Deserialize(ev.Event));
                });
        }

        private async Task CreateSubscription(string subscription, string eventTypeStream, PersistentSubscriptionSettingsBuilder settings)
        {
            try
            {
                await _conn.CreatePersistentSubscriptionAsync(eventTypeStream, subscription, settings, _credentials);
            }
            catch
            {
                // Already exists
            }
        }
    }
}