using System;

namespace Meetup.Domain.Events
{
    public class MeetupRsvpOpenedEvent : MeetupEvent
    {
        public int NumberOfSpots { get; }

        public MeetupRsvpOpenedEvent(Guid meetupId, int numberOfSpots = 0) : base(meetupId)
        {
            NumberOfSpots = numberOfSpots;
        }
    }
}