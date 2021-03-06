using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace Meetup.Api
{
    internal class AsyncMessagingService : BackgroundService
    {
        private readonly MessageDispatcherService _dispatcher;
        private readonly ILogger<MessageDispatcherService> _logger;

        public AsyncMessagingService(MessageDispatcherService dispatcher, ILogger<MessageDispatcherService> logger)
        {
            _dispatcher = dispatcher;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting Message Dispatcher Background Service");
            _dispatcher.Start();
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stopping Message Dispatcher Background Service");
            return Task.CompletedTask;
        }
    }

    public static class AsyncMessagingExtensions
    {
        public static IMessageHandlerBuilder AddAsyncMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            var subscription = configuration.GetValue("subscription_name", "default");
            var options = new AsyncMessagingOptions { Subscription = subscription };
            var registry = new MessageHandlerRegistry();
            var factory = new MessageHandlerFactory(services);
            services
                .AddSingleton(options)
                .AddSingleton(registry)
                .AddSingleton(factory)
                .AddSingleton<MessageDispatcherService>()
                .AddHostedService<AsyncMessagingService>();

            return new MessageHandlerBuilder(services, registry);
        }
    }
}