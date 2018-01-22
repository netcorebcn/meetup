using System;

namespace Meetup.Domain.Events
{
    public abstract class MeetupEvent
    {
        public Guid MeetupId { get; }

        protected MeetupEvent(Guid meetupId)
        {
            MeetupId = meetupId;
        } 
    }
}