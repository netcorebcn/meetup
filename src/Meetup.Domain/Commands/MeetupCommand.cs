
using System;

namespace Meetup.Domain.Commands
{
    public abstract class MeetupCommand
    {
        public Guid MeetupId { get; }

        protected MeetupCommand(Guid meetupId)
        {
            MeetupId = meetupId;
        } 
    }
}
