using System;
using System.Collections.Generic;
using Meetup.Domain.Events;

namespace Meetup.Domain
{
    public class MeetupAggregate
    {
        private List<object> _pendingEvents = new List<object>();
        private MeetupState _state = MeetupState.Empty;

        public void Publish()
        {
            TryRaiseEvent(new MeetupRsvpOpenedEvent());
        }

        public void AcceptRsvp()
        {
            TryRaiseEvent(new MeetupRsvpAcceptedEvent());
        }
        
        public void DeclineRsvp()
        {
            TryRaiseEvent(new MeetupRsvpDeclinedEvent());
        }

        public void Cancel()
        {
            TryRaiseEvent(new MeetupCanceledEvent());
        }

        public void Close()
        {
            TryRaiseEvent(new MeetupRsvpClosedEvent());
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
            if (_state.CanRaiseEvent(@event.GetType()))
            {
                _pendingEvents.Add(@event);
                Apply(@event);
            }
        }
    }
}
