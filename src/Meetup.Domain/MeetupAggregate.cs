using System;
using System.Collections.Generic;
using Meetup.Domain.Commands;
using Meetup.Domain.Events;

namespace Meetup.Domain
{
    public class MeetupAggregate
    {
        private List<object> _pendingEvents = new List<object>();
        
        public MeetupState State { get; private set; } = MeetupState.Empty;

        public Guid MeetupId { get; }

        public MeetupAggregate(Guid meetupId) => MeetupId = meetupId;

        public void Publish(MeetupPublishCommand command) => 
            TryRaiseEvent(new MeetupRsvpOpenedEvent(MeetupId, command.NumberOfSpots));

        public void AcceptRsvp(Guid memberId) => 
            TryRaiseEvent(new MeetupRsvpAcceptedEvent(MeetupId, memberId));

        public void DeclineRsvp(Guid memberId) => 
            TryRaiseEvent(new MeetupRsvpDeclinedEvent(MeetupId, memberId));

        public void UpdateMeetupSpots(int numberOfSpots) => 
            TryRaiseEvent(new MeetupNumberOfSpotsChangedEvent(MeetupId, numberOfSpots));

        public void Cancel() => 
            TryRaiseEvent(new MeetupCanceledEvent(MeetupId));

        public void Close() => 
            TryRaiseEvent(new MeetupRsvpClosedEvent(MeetupId));

        public void TakeAttendance(Guid memberId) => 
            TryRaiseEvent(new MeetupMemberWentEvent(MeetupId, memberId));

        public void Apply(object @event)
        {
            switch(@event)
            {
                case MeetupRsvpOpenedEvent opened:
                    State = MeetupState.Published;
                    break;
                case MeetupCanceledEvent canceled:
                    State = MeetupState.Cancelled;
                    break;
                case MeetupRsvpClosedEvent closed:
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
