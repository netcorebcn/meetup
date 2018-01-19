using System;
using System.Collections.Generic;

namespace Meetup.Domain
{
    public class MeetupAggregate
    {
        public List<object> PendingEvents { get;} = new List<object>();

        public void Publish()
        {
            RaiseEvent(new MeetupPublishedEvent());
        }

        private void RaiseEvent(object @event) 
        {
            PendingEvents.Add(@event);
            Apply(@event);
        }

        public void Apply(object @event)
        {
            
        }
    }
}
