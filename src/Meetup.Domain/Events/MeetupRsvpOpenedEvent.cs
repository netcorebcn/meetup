using System;

namespace Meetup.Domain.Events
{
    public class MeetupRsvpOpenedEvent : MeetupEvent
    {
        public MeetupRsvpOpenedEvent(Guid meetupId) : base(meetupId)
        {
        }
    }
}