using System;

namespace Meetup.Domain
{
    public static class Events
    {
        public class MeetupCreated
        {
            public Guid Id { get; }
            public string Title { get; }
            public MeetupCreated(Guid id, string title)
            {
                Id = id;
                Title = title;
            }
        }

        public class MeetupTitleUpdated
        {
            public Guid Id { get; }
            public string Title { get; }
            public MeetupTitleUpdated(Guid id, string title)
            {
                Id = id;
                Title = title;
            }
        }

        public class MeetupLocationUpdated
        {
            public Guid Id { get; }
            public string Location { get; }
            public MeetupLocationUpdated(Guid id, string location)
            {
                Id = id;
                Location = location;
            }
        }

        public class MeetupTimeUpdated
        {
            public Guid Id { get; }
            public DateTime Start { get; }
            public DateTime End { get; }
            public MeetupTimeUpdated(Guid id, DateTime start, DateTime end)
            {
                Id = id;
                Start = start;
                End = end;
            }
        }

        public class MeetupNumberOfSeatsUpdated
        {
            public Guid Id { get; }
            public int NumberOfSeats { get; }

            public MeetupNumberOfSeatsUpdated(Guid id, int numberOfSeats)
            {
                Id = id;
                NumberOfSeats = numberOfSeats;
            }
        }

        public class MeetupPublished
        {
            public Guid Id { get; }

            public MeetupPublished(Guid id) => Id = id;
        }

        public class MeetupClosed
        {
            public Guid Id { get; }
            public MeetupClosed(Guid id) => Id = id;
        }

        public class MeetupCanceled
        {
            public Guid Id { get; }
            public MeetupCanceled(Guid id) => Id = id;
        }

        public class RSVPAccepted
        {
            public Guid MeetupId { get; }
            public Guid MemberId { get; }
            public DateTime AcceptedAt { get; }

            public RSVPAccepted(Guid meetupId, Guid memberId, DateTime acceptedAt)
            {
                MeetupId = meetupId;
                AcceptedAt = acceptedAt;
            }
        }

        public class RSVPRejected
        {
            public Guid MeetupId { get; }
            public Guid MemberId { get; }
            public DateTime RejectedAt { get; }

            public RSVPRejected(Guid meetupId, Guid memberId, DateTime rejectedAt)
            {
                MeetupId = meetupId;
                MemberId = memberId;
                RejectedAt = rejectedAt;
            }
        }
    }
}