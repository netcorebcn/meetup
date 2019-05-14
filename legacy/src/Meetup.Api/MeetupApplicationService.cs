using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Meetup.Domain;
using Meetup.Domain.Commands;

namespace Meetup.Api
{
    public class MeetupApplicationService
    {
        DocumentStore _documentStore;
        public object GetState(Guid id)
        {
            var aggregate = new MeetupGeneral(id);
            return aggregate.State.ToString();
        }

        public string Publish(MeetupPublishCommand command)
        {
            var aggregate = new MeetupGeneral(command.MeetupId);
            aggregate.Publish(command);
            return aggregate.State.ToString();
        }

        public string Close(Guid id)
        {
            var aggregate = new MeetupGeneral(id);
            aggregate.Close();
            return aggregate.State.ToString();
        }

        public string Cancel(Guid id)
        {
            var aggregate = new MeetupGeneral(id);
            aggregate.Cancel();
            return aggregate.State.ToString();
        }

        public async Task<(bool, string)> TryAcceptRsvp(AcceptRsvpCommand command)
        {
            using (var session = _documentStore.OpenSession())
            {
                var events = (await session.Events.FetchStreamAsync(command.MeetupId)).Select(x => x.Data).ToList();

                var aggregate = WriteAggregate.Create(events);
                var result = aggregate.AcceptRsvp(command);

                session.Events.Append(command.MeetupId, aggregate.PendingEvents.ToArray());
                await session.SaveChangesAsync();

                return result;
            }
        }
    }
}