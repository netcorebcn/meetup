using System;

namespace Meetup.Domain.Events
{
    public class MeetupCanceledEvent : MeetupEvent
    {
        public MeetupCanceledEvent(Guid meetupId) : base(meetupId)
        {

        }
    }
}