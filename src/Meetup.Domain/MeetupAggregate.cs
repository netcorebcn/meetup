using System;
using System.Collections.Generic;
using Meetup.Domain.Events;

namespace Meetup.Domain
{
    public class MeetupAggregate
    {
        private List<object> _pendingEvents = new List<object>();
        private MeetupState _state = MeetupState.Empty;

        public Guid MeetupId { get; }

        public MeetupAggregate(Guid meetupId) => MeetupId = meetupId;

        public void Publish()
        {
            TryRaiseEvent(new MeetupRsvpOpenedEvent(MeetupId));
        }

        public void AcceptRsvp(Guid memberId)
        {
            TryRaiseEvent(new MeetupRsvpAcceptedEvent(MeetupId, memberId));
        }
        
        public void DeclineRsvp(Guid memberId)
        {
            TryRaiseEvent(new MeetupRsvpDeclinedEvent(MeetupId, memberId));
        }

        public void Cancel()
        {
            TryRaiseEvent(new MeetupCanceledEvent(MeetupId));
        }

        public void Close()
        {
            TryRaiseEvent(new MeetupRsvpClosedEvent(MeetupId));
        }

        public void Apply(object @event)
        {
            switch(@event)
            {
                case MeetupRsvpOpenedEvent opened:
                    _state = MeetupState.Published;
                    break;
                case MeetupRsvpAcceptedEvent rsvpAccepted:
                    break;
                case MeetupRsvpDeclinedEvent rsvpDeclined:
                    break;
                case MeetupCanceledEvent canceled:
                    _state = MeetupState.Cancelled;
                    break;
                case MeetupRsvpClosedEvent closed:
                    _state = MeetupState.Closed;
                    break;
            }            
        }

        public IEnumerable<object> GetPendingEvents() => _pendingEvents;

        public void ClearEvent() => _pendingEvents.Clear();

        private void TryRaiseEvent(object @event) 
        {
            if (@event != null && _state.CanRaiseEvent(@event.GetType()))
            {
                _pendingEvents.Add(@event);
                Apply(@event);
            }
        }
    }
}
