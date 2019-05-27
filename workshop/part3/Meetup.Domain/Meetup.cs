using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable enable
namespace Meetup.Domain
{
    public class Meetup
    {
        public MeetupId Id { get; private set; } = MeetupId.None;
        public MeetupTitle Title { get; private set; } = MeetupTitle.None;
        public Address Location { get; private set; } = Address.None;
        public SeatsNumber NumberOfSeats { get; private set; } = SeatsNumber.None;
        public DateTimeRange TimeRange { get; private set; } = DateTimeRange.None;
        public MeetupState State { get; private set; } = MeetupState.Created;
        private readonly List<object> _events = new List<object>();
        public IEnumerable<object> Events => _events.AsEnumerable();
        private readonly Dictionary<MemberId, DateTime> _membersGoing = new Dictionary<MemberId, DateTime>();
        public IReadOnlyDictionary<MemberId, DateTime> MembersGoing => _membersGoing;
        private readonly Dictionary<MemberId, DateTime> _membersNotGoing = new Dictionary<MemberId, DateTime>();
        public IReadOnlyDictionary<MemberId, DateTime> MembersNotGoing => _membersNotGoing;

        public Meetup(MeetupId id, MeetupTitle title, Address location, SeatsNumber numberOfSeats, DateTimeRange timeRange, MeetupState state)
        {
            Id = id;
            Title = title;
            Location = location;
            NumberOfSeats = numberOfSeats;
            TimeRange = timeRange;
            State = state;
            EnsureInvariants();
        }

        public Meetup(MeetupId id, MeetupTitle title) =>
            Apply(new Events.MeetupCreated(id, title));

        public void UpdateNumberOfSeats(SeatsNumber number) =>
            Apply(new Events.MeetupNumberOfSeatsUpdated(Id, number));

        public void UpdateLocation(Address location) =>
            Apply(new Events.MeetupLocationUpdated(Id, location));

        public void UpdateTime(DateTimeRange timeRange) =>
            Apply(new Events.MeetupTimeUpdated(Id, timeRange.Start, timeRange.End));

        public void UpdateTitle(MeetupTitle title) =>
            Apply(new Events.MeetupTitleUpdated(Id, title));

        public void Publish() =>
            Apply(new Events.MeetupPublished(Id));

        public void Cancel() =>
            Apply(new Events.MeetupCanceled(Id));

        public void Close() =>
            Apply(new Events.MeetupClosed(Id));

        public void AcceptRSVP(MemberId memberId, DateTime acceptedAt) =>
            Apply(new Events.RSVPAccepted(Id, memberId, acceptedAt));

        public void RejectRSVP(MemberId memberId, DateTime rejectedAt) =>
            Apply(new Events.RSVPRejected(Id, memberId, rejectedAt));


        private void When(object @event)
        {
            switch (@event)
            {
                case Events.MeetupCreated ev:
                    Id = MeetupId.From(ev.Id);
                    Title = MeetupTitle.From(ev.Title);
                    State = MeetupState.Created;
                    break;
                case Events.MeetupPublished ev:
                    State = MeetupState.Published;
                    break;
                case Events.MeetupCanceled ev:
                    State = MeetupState.Canceled;
                    break;
                case Events.MeetupClosed ev:
                    State = MeetupState.Closed;
                    break;
                case Events.MeetupNumberOfSeatsUpdated ev:
                    NumberOfSeats = SeatsNumber.From(ev.NumberOfSeats);
                    break;
                case Events.MeetupTimeUpdated ev:
                    TimeRange = DateTimeRange.From(ev.Start, ev.End);
                    break;
                case Events.MeetupTitleUpdated ev:
                    Title = MeetupTitle.From(ev.Title);
                    break;
                case Events.MeetupLocationUpdated ev:
                    Location = Address.From(ev.Location);
                    break;
                case Events.RSVPAccepted ev:
                    _membersGoing.Add(MemberId.From(ev.MemberId), ev.AcceptedAt);
                    break;
                case Events.RSVPRejected ev:
                    _membersNotGoing.Add(MemberId.From(ev.MemberId), ev.RejectedAt);
                    break;
            }
        }

        private void EnsureInvariants()
        {
            var valid = Id != MeetupId.None && Title != MeetupTitle.None &&
            State switch
            {
                MeetupState state when state != MeetupState.Created =>
                    Location != Address.None
                    && NumberOfSeats != SeatsNumber.None
                    && TimeRange != DateTimeRange.None,
                _ => true
            };

            if (!valid)
            {
                throw new MeetupDomainException($"Invalid state {State}");
            }
        }

        private void Apply(object @event)
        {
            State.EnsureCanRaiseEvent(@event);
            When(@event);
            EnsureInvariants();
            _events.Add(@event);
        }
    }
}