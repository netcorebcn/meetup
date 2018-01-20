using System;
using System.Collections.Generic;
using Meetup.Domain.Events;

namespace Meetup.Domain
{
    public class MeetupAggregate
    {
        public List<object> PendingEvents { get; } = new List<object>();

        public void Publish()
        {
            RaiseEvent(new MeetupRsvpOpenedEvent());
        }

        public void AcceptRsvp()
        {
            RaiseEvent(new MeetupRsvpAcceptedEvent());
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
                    break;
                case MeetupRsvpAcceptedEvent rsvpAccepted:
                    break;
                case MeetupRsvpDeclinedEvent rsvpDeclined:
                    break;
                case MeetupCanceledEvent canceled:
                    break;
                case MeetupRsvpClosedEvent closed:
                    break;
            }            
        }

        private void RaiseEvent(object @event) 
        {
            PendingEvents.Add(@event);
            Apply(@event);
        }
    }
}
