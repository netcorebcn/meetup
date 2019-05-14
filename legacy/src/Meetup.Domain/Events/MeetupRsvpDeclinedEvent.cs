using System;

namespace Meetup.Domain.Events
{
    public class MeetupRsvpDeclinedEvent : MeetupEvent
    {
        public Guid MemberId { get; }

        public MeetupRsvpDeclinedEvent(Guid meetupId, Guid memberId) : base(meetupId)
        {
            MemberId = memberId;
        }
    }
}