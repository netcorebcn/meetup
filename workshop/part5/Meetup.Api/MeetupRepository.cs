using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Meetup.Domain;
using Newtonsoft.Json;

namespace Meetup.Api
{
    public class MeetupRepository : IMeetupRepository
    {
        private IEventStoreConnection _eventStore;

        public MeetupRepository(IEventStoreConnection eventstore) => _eventStore = eventstore;

        public async Task<MeetupAggregate> Get(Guid id)
        {
            var streamName = $"{typeof(MeetupAggregate).Name}-{id}";

            // var page = await _eventStore.ReadStreamEventsForwardAsync(streamName, 0, 1024, false);
            // var events = page.Events.Select(resolvedEvent => resolvedEvent.Deserialze()).ToArray();
            // return MeetupAggregate.Build(MeetupId.From(id), events);
            return await Task.FromResult<MeetupAggregate>(null);
        }

        public async Task Save(MeetupAggregate meetup)
        {
            var streamName = $"{typeof(MeetupAggregate).Name}-{meetup.Id}";
            await Task.CompletedTask;

            // var eventStreamState = await session.Events.FetchStreamStateAsync(meetup.Id);
            // var expectedVersion = (eventStreamState?.Version ?? 0) + meetup.Events.Count();

            // var preparedEvents = meetup.Events.Select(@event =>
            //     new EventData(
            //         Guid.NewGuid(),
            //         TypeMapper.GetTypeName(@event.GetType()),
            //         true,
            //         Serialize(@event),
            //         Serialize(
            //             new EventMetadata
            //             {
            //                 ClrType = @event.GetType().FullName
            //             }
            //         ))).ToArray();

            // return connection.AppendToStreamAsync(
            //     streamName,
            //     version,
            //     preparedEvents
            // );

            // await PersistProjections(session, meetup.Id, pendingEvents);
        }

        // private async Task PersistProjections(IDocumentSession session, Guid streamId, object[] events)
        // {
        //     await PersistProjections<AttendantsReadModel, AttendantsProjection>(session, streamId, events);
        //     await PersistProjections<MeetupReadModel, MeetupProjection>(session, streamId, events);
        // }

        // private async Task PersistProjections<TReadModel, TProjection>(IDocumentSession session, Guid streamId, object[] events)
        //     where TProjection : IProjection<TReadModel>, new()
        //     where TReadModel : new()
        // {
        //     var readModel = (await session.LoadAsync<TReadModel>(streamId)) ?? new TReadModel();
        //     var newReadModel = new TProjection().Project(readModel, events);
        //     session.Store(newReadModel);
        // }
    }
}