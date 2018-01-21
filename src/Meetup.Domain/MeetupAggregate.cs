using System;
using System.Collections.Generic;
using Meetup.Domain.Events;

namespace Meetup.Domain
{
    public class MeetupAggregate
    {
        private List<object> _pendingEvents = new List<object>();
        private bool _cancelled;
        private bool _opened;

        public void Publish()
        {
            if (!_cancelled)
            {
                RaiseEvent(new MeetupRsvpOpenedEvent());
            }
        }

        public void AcceptRsvp()
        {
            if(!_cancelled && _opened)
            {
                RaiseEvent(new MeetupRsvpAcceptedEvent());
            } 
        }
        
        public void DeclineRsvp()
        {
            RaiseEvent(new MeetupRsvpDeclinedEvent());
        }

        public void Cancel()
        {
            RaiseEvent(new MeetupCanceledEvent());
        }

        public void Close()
        {
            RaiseEvent(new MeetupRsvpClosedEvent());
        }

        public void Apply(object @event)
        {
            switch(@event)
            {
                case MeetupRsvpOpenedEvent opened:
                    _opened = true;
                    break;
                case MeetupRsvpAcceptedEvent rsvpAccepted:
                    break;
                case MeetupRsvpDeclinedEvent rsvpDeclined:
                    break;
                case MeetupCanceledEvent canceled:
                    _cancelled = true;
                    break;
                case MeetupRsvpClosedEvent closed:
                    _opened = false;
                    break;
            }            
        }

        public IEnumerable<object> GetPendingEvents() => _pendingEvents;

        public void ClearEvent() => _pendingEvents.Clear();

        private void RaiseEvent(object @event) 
        {
            _pendingEvents.Add(@event);
            Apply(@event);
        }
    }
}
