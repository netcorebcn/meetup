using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Microsoft.Extensions.Hosting;

namespace Meetup.Api
{
    public class EventStoreService : IHostedService
    {
        private readonly IEventStoreConnection _esConnection;
        private readonly SubscriptionManager _subscriptionManager;

        public EventStoreService(IEventStoreConnection esConnection, SubscriptionManager subscriptionManager)
        {
            _esConnection = esConnection;
            _subscriptionManager = subscriptionManager;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _esConnection.ConnectAsync();
            await _subscriptionManager.Start();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscriptionManager.Stop();
            _esConnection.Close();
            return Task.CompletedTask;
        }
    }
}