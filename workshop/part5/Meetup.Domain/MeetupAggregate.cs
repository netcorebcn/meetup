using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable enable
namespace Meetup.Domain
{
    public class MeetupAggregate
    {
        private MeetupTitle Title = MeetupTitle.None;
        private Address Location = Address.None;
        private SeatsNumber NumberOfSeats = SeatsNumber.None;
        private DateTimeRange TimeRange = DateTimeRange.None;
        private MeetupState State = MeetupState.Created;
        public MeetupId Id { get; private set; } = MeetupId.None;
        private readonly List<object> _events = new List<object>();
        public IEnumerable<object> Events => _events.AsEnumerable();

        public static MeetupAggregate Build(MeetupId id, params object[] events)
        {
            var meetup = new MeetupAggregate(id);
            events.ToList().ForEach(ev =>
            {
                meetup.State.EnsureCanRaiseEvent(ev.GetType());
                meetup.When(ev);
                meetup.EnsureInvariants();
            });
            return meetup;
        }

        private MeetupAggregate(MeetupId id) => Id = id;

        public MeetupAggregate(MeetupId id, MeetupTitle title) =>
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
            State.EnsureCanRaiseEvent(@event.GetType());
            When(@event);
            EnsureInvariants();
            _events.Add(@event);
        }
    }
}