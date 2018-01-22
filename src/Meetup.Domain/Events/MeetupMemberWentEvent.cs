using System;

namespace Meetup.Domain.Events
{
    public class MeetupMemberWentEvent : MeetupEvent
    {
        public Guid MemberId { get; }

        public MeetupMemberWentEvent(Guid meetupId, Guid memberId) : base(meetupId)
        {
            MemberId = memberId;
        }
    }
}