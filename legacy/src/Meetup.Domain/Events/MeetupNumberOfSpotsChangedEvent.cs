
using System;

namespace Meetup.Domain.Events
{
    public class MeetupNumberOfSpotsChangedEvent: MeetupEvent
    {
        public int NumberOfSpots { get; }

        public MeetupNumberOfSpotsChangedEvent(Guid meetupId, int numberOfSpots) : base(meetupId)
        {
            NumberOfSpots = numberOfSpots;
        }
    }
}