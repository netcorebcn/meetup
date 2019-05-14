using System;

namespace Meetup.Domain
{
    public class AcceptRsvpCommand
    {
        public Guid UserId;

        public Guid MeetupId;
    }
}