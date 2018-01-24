using System;
using Meetup.Domain;
using Meetup.Domain.Commands;

namespace Meetup.Api
{
    public class MeetupApplicationService
    {
        public object GetState(Guid id)
        {
            var aggregate = new MeetupAggregate(id);
            return aggregate.State.ToString();
        }

        public string Publish(MeetupPublishCommand command)
        {
            var aggregate = new MeetupAggregate(command.MeetupId);
            aggregate.Publish(command);
            return aggregate.State.ToString();
        }

        public string Close(Guid id)
        {
            var aggregate = new MeetupAggregate(id);
            aggregate.Close();
            return aggregate.State.ToString();
        }

        public string Cancel(Guid id)
        {
            var aggregate = new MeetupAggregate(id);
            aggregate.Cancel();
            return aggregate.State.ToString();
        }
    }
}