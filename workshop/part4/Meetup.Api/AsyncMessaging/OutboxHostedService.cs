using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Marten;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Meetup.Api
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly IDocumentStore _store;
        private readonly IBus _bus;
        private readonly ILogger<OutboxProcessor> _logger;

        public OutboxProcessor(IDocumentStore store, IBus bus, ILogger<OutboxProcessor> logger)
        {
            _store = store;
            _bus = bus;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var session = _store.OpenSession();
                var outboxEvents = await session.Query<OutboxEvent>().Take(100).ToListAsync();

                foreach (var ev in outboxEvents)
                {
                    _logger.LogDebug($"Sending outbox,type {ev.EventType} and data {ev.Data}");
                    dynamic typedEvent = ev.Data;
                    await _bus.PublishAsync(typedEvent);
                    session.Delete(ev);
                }

                await session.SaveChangesAsync();
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}