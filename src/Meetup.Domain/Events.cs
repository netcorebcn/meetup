using System;

namespace Meetup.Domain
{
    public static class Events
    {
        public class MeetupRsvpOpened
        {
            public Guid Id { get; }
            public int NumberOfSpots { get; }

            public MeetupRsvpOpened(Guid id, int numberOfSpots)
            {
                Id = id;
                NumberOfSpots = numberOfSpots;
            }
        }

        public class MeetupNumberOfSpotsChanged
        {
            public Guid Id { get; }
            public int NumberOfSpots { get; }

            public MeetupNumberOfSpotsChanged(Guid id, int numberOfSpots)
            {
                Id = id;
                NumberOfSpots = numberOfSpots;
            }
        }

        public class MeetupRsvpClosed
        {
            public Guid Id { get; }
            public MeetupRsvpClosed(Guid id) => Id = id;
        }

        public class MeetupCanceled
        {
            public Guid Id { get; }
            public MeetupCanceled(Guid id) => Id = id;
        }

        public class MeetupMemberWent
        {
            public Guid Id { get; }
            public Guid MemberId { get; }

            public MeetupMemberWent(Guid id, Guid memberId)
            {
                Id = id;
                MemberId = memberId;
            }
        }

        public class MeetupRsvpAccepted
        {
            public Guid Id { get; }
            public Guid MemberId { get; }

            public MeetupRsvpAccepted(Guid id, Guid memberId)
            {
                Id = id;
                MemberId = memberId;
            }
        }

        public class MeetupRsvpDeclined
        {
            public Guid Id { get; }
            public Guid MemberId { get; }

            public MeetupRsvpDeclined(Guid id, Guid memberId)
            {
                Id = id;
                MemberId = memberId;
            }
        }
    }
}