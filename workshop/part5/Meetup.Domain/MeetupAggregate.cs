using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable enable
namespace Meetup.Domain
{
    public class MeetupAggregate : Aggregate<MeetupId>
    {
        private MeetupTitle Title = MeetupTitle.None;
        private Address Location = Address.None;
        private SeatsNumber NumberOfSeats = SeatsNumber.None;
        private DateTimeRange TimeRange = DateTimeRange.None;
        private MeetupState State = MeetupState.Created;

        private MeetupAggregate() { }

        public void Create(MeetupId id, MeetupTitle title) =>
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

        protected override void When(object @event)
        {
            State.EnsureCanRaiseEvent(@event.GetType());
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

        protected override void EnsureInvariants()
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
    }
}