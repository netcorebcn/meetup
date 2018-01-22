using System;

namespace Meetup.Domain.Events
{
    public class MeetupRsvpAcceptedEvent : MeetupEvent
    {
        public Guid MemberId { get; }

        public MeetupRsvpAcceptedEvent(Guid meetupId, Guid memberId) : base(meetupId)
        {
            MemberId = memberId;
        }
    }
}