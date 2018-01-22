using System;

namespace Meetup.Domain.Events
{
    public class MeetupRsvpClosedEvent : MeetupEvent
    {
        public MeetupRsvpClosedEvent(Guid meetupId) : base(meetupId)
        {
        }
    }
}