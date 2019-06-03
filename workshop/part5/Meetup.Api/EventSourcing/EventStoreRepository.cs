using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Meetup.Domain;

namespace Meetup.Api
{
    internal class EventStoreRepository : IEventStoreRepository
    {
        public static readonly string EventClrTypeHeader = "EventClrTypeName";
        public static readonly string AggregateClrTypeHeader = "AggregateClrTypeName";
        private const int ReadPageSize = 500;

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly EventDeserializer _eventDeserializer;

        public EventStoreRepository(IEventStoreConnection eventStoreConnection, EventDeserializer eventDeserializer)
        {
            _eventStoreConnection = eventStoreConnection;
            _eventDeserializer = eventDeserializer;
        }

        public async Task<TAggregate> Get<TAggregate, TId>(TId id) where TAggregate : Aggregate<TId>
        {
            var aggregate = (TAggregate)Activator.CreateInstance(typeof(TAggregate), true);
            var eventStream = await GetEventStream<TAggregate, TId>(id);
            aggregate.Load(eventStream.ToArray());
            return aggregate;
        }

        public async Task Save<TAggregate, TId>(TAggregate aggregate) where TAggregate : Aggregate<TId>
        {
            var streamName = StreamName($"{aggregate.GetType().Name }-{aggregate.Id}");
            var eventsToSave = aggregate.Events.Select(
                ev => ToEventData(
                    Guid.NewGuid(),
                    ev,
                    new Dictionary<string, string> {
                        { AggregateClrTypeHeader, aggregate.GetType().AssemblyQualifiedName }
                    }));

            await _eventStoreConnection.AppendToStreamAsync(streamName, aggregate.Version, eventsToSave);
            aggregate.ClearPendingEvents();

            EventData ToEventData(Guid eventId, object evnt, IDictionary<string, string> headers)
            {
                var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt, SerializerSettings));
                var eventHeaders = new Dictionary<string, string>(headers)
                {
                    {EventClrTypeHeader, evnt.GetType().AssemblyQualifiedName}
                };
                var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders, SerializerSettings));
                var typeName = evnt.GetType().Name;
                return new EventData(eventId, typeName, true, data, metadata);
            }
        }

        public async Task<IEnumerable<object>> GetEventStream<TAggregate, TId>(TId id) where TAggregate : Aggregate<TId>
        {
            var streamName = StreamName($"{typeof(TAggregate).Name }-{id}");

            var eventNumber = 0;
            StreamEventsSlice currentSlice;

            var eventStream = new List<object>();
            do
            {
                currentSlice = await _eventStoreConnection.ReadStreamEventsForwardAsync(streamName, eventNumber, ReadPageSize, false);

                // if (currentSlice.Status == SliceReadStatus.StreamNotFound)
                //     throw new AggregateNotFoundException(id, typeof(TAggregate));

                // if (currentSlice.Status == SliceReadStatus.StreamDeleted)
                //     throw new AggregateDeletedException(id, typeof(TAggregate));

                eventNumber = (int)currentSlice.NextEventNumber;

                foreach (var resolvedEvent in currentSlice.Events)
                {
                    eventStream.Add(_eventDeserializer.Deserialize(resolvedEvent.Event));
                }

            } while (!currentSlice.IsEndOfStream);

            return eventStream;
        }

        private string StreamName(string streamName)
        {
            var sp = streamName.Split(new[] { '-' }, 2);
            return $"{sp[0]}-{sp[1].Replace("-", "")}";
        }
    }
}