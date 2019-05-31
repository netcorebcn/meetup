using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Meetup.Domain;
using Newtonsoft.Json;

namespace Meetup.Api
{
    public class MeetupRepository : IMeetupRepository
    {
        private IDocumentStore _eventStore;

        public MeetupRepository(IDocumentStore eventstore) => _eventStore = eventstore;

        public async Task<MeetupAggregate> Get(Guid id)
        {
            using var session = _eventStore.OpenSession();
            var stream = await session.Events.FetchStreamAsync(id);
            var events = stream.Select(@event => @event.Data).ToArray();
            return MeetupAggregate.Build(MeetupId.From(id), events);
        }

        public async Task Save(MeetupAggregate meetup)
        {
            using var session = _eventStore.OpenSession();
            var eventStreamState = await session.Events.FetchStreamStateAsync(meetup.Id);
            var expectedVersion = (eventStreamState?.Version ?? 0) + meetup.Events.Count();
            var pendingEvents = meetup.Events.ToArray();

            session.Events.Append(meetup.Id, expectedVersion, pendingEvents);
            await PersistProjections(session, meetup.Id, pendingEvents);
            StoreOutboxEvents(session, meetup.Id, pendingEvents);

            await session.SaveChangesAsync();
        }

        private void StoreOutboxEvents(IDocumentSession session, MeetupId id, object[] pendingEvents)
        {
            session.Store<OutboxEvent>(pendingEvents.Select(ev => new OutboxEvent
            {
                Id = Guid.NewGuid(),
                StreamId = id,
                Data = ev,
                EventType = ev.GetType().Name,
                ClrEventType = ev.GetType().FullName

            }).ToArray());
        }

        private async Task PersistProjections(IDocumentSession session, Guid streamId, object[] events)
        {
            await PersistProjections<AttendantsReadModel, AttendantsProjection>(session, streamId, events);
            await PersistProjections<MeetupReadModel, MeetupProjection>(session, streamId, events);
        }

        private async Task PersistProjections<TReadModel, TProjection>(IDocumentSession session, Guid streamId, object[] events)
            where TProjection : IProjection<TReadModel>, new()
            where TReadModel : new()
        {
            var readModel = (await session.LoadAsync<TReadModel>(streamId)) ?? new TReadModel();
            var newReadModel = new TProjection().Project(readModel, events);
            session.Store(newReadModel);
        }
    }
}