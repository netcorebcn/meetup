using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Meetup.Domain
{
    public class Meetup
    {
        public MeetupId Id { get; }
        public MeetupTitle Title { get; private set; }
        public Address Location { get; private set; }
        public SeatsNumber NumberOfSeats { get; private set; }
        public DateTimeRange TimeRange { get; private set; }
        public MeetupState State { get; private set; }

        public Meetup(MeetupId id, MeetupTitle title)
        {
            Id = id;
            Title = title;
            Location = Address.None;
            NumberOfSeats = SeatsNumber.None;
            TimeRange = DateTimeRange.None;
            State = MeetupState.Created;
            EnsureInvariants();
        }

        public void UpdateNumberOfSeats(SeatsNumber number)
        {
            NumberOfSeats = number;
            EnsureInvariants();
        }

        public void UpdateLocation(Address location)
        {
            Location = location;
            EnsureInvariants();
        }

        public void UpdateTime(DateTimeRange timeRange)
        {
            TimeRange = timeRange;
            EnsureInvariants();
        }
        public void UpdateTitle(MeetupTitle title)
        {
            Title = title;
            EnsureInvariants();
        }

        public void Publish()
        {
            var previous = State;
            State = MeetupState.Published;
            EnsureInvariants(previous);
        }

        public void Cancel()
        {
            var previous = State;
            State = MeetupState.Canceled;
            EnsureInvariants(previous);
        }

        public void Close()
        {
            var previous = State;
            State = MeetupState.Closed;
            Console.WriteLine($"MeetupState previous {previous}, Current {State}");
            EnsureInvariants(previous);
        }

        private void EnsureInvariants() => EnsureInvariants(State);

        private void EnsureInvariants(MeetupState previous)
        {
            var valid = Id != MeetupId.None && Title != MeetupTitle.None &&
            State switch
            {
                MeetupState.Published => RequiredFields(Published),
                MeetupState.Canceled => RequiredFields(Canceled),
                MeetupState.Closed => RequiredFields(Closed),
                _ => true
            };

            if (!valid)
            {
                throw new MeetupDomainException($"Invalid state {State}");
            }

            bool RequiredFields(Func<MeetupState[]> states) =>
                states().Any(s => s == previous)
                && Location != Address.None
                && NumberOfSeats != SeatsNumber.None
                && TimeRange != DateTimeRange.None;

            MeetupState[] Published() => new[] { MeetupState.Published, MeetupState.Created };
            MeetupState[] Canceled() => new[] { MeetupState.Canceled, MeetupState.Published };
            MeetupState[] Closed() => new[] { MeetupState.Closed, MeetupState.Published };
        }
    }

    public enum MeetupState
    {
        Created,
        Published,
        Canceled,
        Closed
    }
}