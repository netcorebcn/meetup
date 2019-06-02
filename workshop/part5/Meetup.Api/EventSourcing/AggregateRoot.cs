using System;
using System.Collections.Generic;

namespace Meetup.Api
{
    public abstract class AggregateRoot
    {
        private readonly List<object> _pendingEvents = new List<object>();

        public Guid Id { get; }
        public int Version { get; private set; } = -1;

        public AggregateRoot() => Id = Guid.NewGuid();

        public AggregateRoot(Guid id) => Id = id;

        public void ApplyEvent(object @event)
        {
            ((dynamic)this).Apply((dynamic)@event);
            Version++;
        }

        public ICollection<object> GetPendingEvents() => _pendingEvents;

        public void ClearPendingEvents() => _pendingEvents.Clear();

        protected void RaiseEvent(object @event)
        {
            ApplyEvent(@event);
            _pendingEvents.Add(@event);
        }
    }
}