using System;
using System.Collections.Generic;

namespace Meetup.Domain
{
    public class MeetupAggregate
    {
        private List<object> _pendingEvents = new List<object>();

        public MeetupState State { get; private set; } = MeetupState.Empty;

        public Guid Id { get; }

        public MeetupAggregate(Guid id) => Id = id;

        public void Publish(int numberOfSpots) =>
            TryRaiseEvent(new Events.MeetupRsvpOpened(Id, numberOfSpots));

        public void AcceptRsvp(Guid memberId) =>
            TryRaiseEvent(new Events.MeetupRsvpAccepted(Id, memberId));

        public void DeclineRsvp(Guid memberId) =>
            TryRaiseEvent(new Events.MeetupRsvpDeclined(Id, memberId));

        public void UpdateMeetupSpots(int numberOfSpots) =>
            TryRaiseEvent(new Events.MeetupNumberOfSpotsChanged(Id, numberOfSpots));

        public void Cancel() =>
            TryRaiseEvent(new Events.MeetupCanceled(Id));

        public void Close() =>
            TryRaiseEvent(new Events.MeetupRsvpClosed(Id));

        public void TakeAttendance(Guid memberId) =>
            TryRaiseEvent(new Events.MeetupMemberWent(Id, memberId));

        public void Apply(object @event)
        {
            switch (@event)
            {
                case Events.MeetupRsvpOpened opened:
                    State = MeetupState.Published;
                    break;
                case Events.MeetupCanceled canceled:
                    State = MeetupState.Cancelled;
                    break;
                case Events.MeetupRsvpClosed closed:
                    State = MeetupState.Closed;
                    break;
            }
        }

        public IEnumerable<object> GetPendingEvents() => _pendingEvents;

        public void ClearEvent() => _pendingEvents.Clear();

        private void TryRaiseEvent(object @event)
        {
            if (@event != null && State.CanRaiseEvent(@event.GetType()))
            {
                _pendingEvents.Add(@event);
                Apply(@event);
            }
        }
    }
}
