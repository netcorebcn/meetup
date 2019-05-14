using System;

namespace Meetup.Domain.Events
{
    public class MeetupEvent
    {
        public readonly Guid MeetupId;
        public readonly DateTime _ts;

        public MeetupEvent(Guid meetupId)
        {
            MeetupId = meetupId;
            _ts = DateTime.UtcNow;
        }
    }
    public class MeetupCreated : MeetupEvent
    {
        public readonly DateTime When;
        public MeetupCreated(Guid meetupId, DateTime when) : base(meetupId)
        {
            When = when;
        }

    }

    public class MeetupCancelled : MeetupEvent
    {
        public MeetupCancelled(Guid meetupId) : base(meetupId)
        {

        }
    }
}