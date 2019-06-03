using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Meetup.Domain;
using Microsoft.Extensions.Configuration;

namespace Meetup.Api
{
    public class SubscriptionManager
    {
        readonly string _name;
        private readonly IEventStoreConnection _connection;
        private readonly IProjection[] _projections;

        private readonly EventDeserializer _deserializer;
        private EventStoreAllCatchUpSubscription _subscription;

        public SubscriptionManager(
            IEventStoreConnection connection,
            EventDeserializer deserializer,
            IConfiguration configuration,
            params IProjection[] projections)
        {
            _connection = connection;
            _name = configuration["subscription_name"] ?? "default";
            _projections = projections;
            _deserializer = deserializer;
        }

        public Task Start()
        {
            var settings = new CatchUpSubscriptionSettings(2000, 500, false, false, _name);
            _subscription = _connection.SubscribeToAllFrom(
                AllCheckpoint.AllStart,
                settings,
                EventAppeared
            );

            return Task.CompletedTask;
        }

        async Task EventAppeared(
            EventStoreCatchUpSubscription _,
            ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.Event.EventType.StartsWith("$")) return;
            var @event = _deserializer.Deserialize(resolvedEvent.Event);
            await Task.WhenAll(_projections.Select(x => x.Project(@event)));
        }

        public void Stop() => _subscription.Stop();
    }
}