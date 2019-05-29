using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Meetup.Domain;

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
            var eventStreamState = await session.Events.FetchStreamStateAsync(meetup.Id.Value);
            var expectedVersion = (eventStreamState?.Version ?? 0) + meetup.Events.Count();
            session.Events.Append(meetup.Id.Value, expectedVersion, meetup.Events.ToArray());
            await session.SaveChangesAsync();
        }
    }
}