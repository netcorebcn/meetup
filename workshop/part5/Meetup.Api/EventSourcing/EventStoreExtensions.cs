using System;
using System.Reflection;
using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meetup.Api
{
    public static class EventSourcingServiceCollectionExtension
    {
        public static IServiceCollection AddEventSourcing(this IServiceCollection services, IConfiguration configuration)
        {
            var esConnection = EventStoreConnection.Create(configuration["eventstore"], ConnectionSettings.Create().KeepReconnecting());

            // var eventDeserializer = new EventDeserializer(typeof(TAggregateRoot).GetTypeInfo().Assembly);
            // var projections = new EventStoreProjectionsClient(options);

            services.AddSingleton(esConnection);
            services.AddHostedService<EventStoreService>();
            // services.AddSingleton(eventDeserializer);
            // services.AddSingleton<IEventStoreProjections>(projections);


            return services;
        }
    }
}