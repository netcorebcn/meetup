using System;
using System.Reflection;
using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Meetup.Api
{
    public static class EventSourcingServiceCollectionExtension
    {
        public static EventDeserializer AddEventSourcing(this IServiceCollection services, IConfiguration configuration)
        {
            var esConnection = EventStoreConnection.Create(
                configuration["eventstore"] ?? "ConnectTo=tcp://admin:changeit@localhost:1113; DefaultUserCredentials=admin:changeit;",
                ConnectionSettings.Create().KeepReconnecting());

            var serviceProvider = services.BuildServiceProvider();
            var eventDeserializer = new EventDeserializer();
            var subscriptionManager = new SubscriptionManager(
                esConnection,
                eventDeserializer,
                configuration,
                serviceProvider.GetRequiredService<ILogger<SubscriptionManager>>(),
                new AttendantsMongoProjection(GetMongoDb, serviceProvider.GetRequiredService<ILogger<AttendantsMongoProjection>>()));

            services.AddSingleton(esConnection);
            services.AddSingleton(eventDeserializer);
            services.AddSingleton<IEventStoreBus, EventStorePersistentSubscription>();
            services.AddSingleton<SubscriptionManager>(subscriptionManager);
            services.AddScoped<IEventStoreRepository, EventStoreRepository>();
            services.AddHostedService<EventStoreService>();
            return eventDeserializer;

            IMongoDatabase GetMongoDb() =>
                new MongoClient(configuration["mongodb"] ?? "mongodb://localhost")
                .GetDatabase(configuration["projectionsdb"] ?? "projectionsdb");
        }
    }
}