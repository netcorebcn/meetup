using System;
using System.Collections.Generic;

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
            State = MeetupState.Published;
            EnsureInvariants();
        }

        public void Cancel()
        {
            State = MeetupState.Canceled;
            EnsureInvariants();
        }

        public void Close()
        {
            State = MeetupState.Closed;
            EnsureInvariants();
        }

        private void EnsureInvariants()
        {
            var valid = Id != null && Title != MeetupTitle.None &&
            State switch
            {
                MeetupState.Published => RequiredFields(),
                MeetupState.Canceled => RequiredFields(),
                MeetupState.Closed => RequiredFields(),
                _ => true
            };

            if (!valid)
            {
                throw new MeetupDomainException($"Invalid state {State}");
            }

            bool RequiredFields() =>
                Location != Address.None
                && NumberOfSeats != SeatsNumber.None
                && TimeRange != DateTimeRange.None;
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