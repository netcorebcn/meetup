using System;
using System.Collections.Generic;
using System.Linq;

namespace Meetup.Domain
{
    public abstract class Aggregate<TId>
    {
        private readonly List<object> _events = new List<object>();
        public IEnumerable<object> Events => _events.AsEnumerable();
        public TId Id { get; protected set; }
        public int Version { get; private set; } = -1;

        public static TAggregate Build<TAggregate>(params object[] events) where TAggregate : Aggregate<TId>
        {
            var aggregate = (TAggregate)Activator.CreateInstance(typeof(TAggregate), true);
            events.ToList().ForEach(ev =>
            {
                aggregate.When(ev);
                aggregate.Version++;
            });
            return aggregate;
        }

        protected abstract void When(object @event);
        protected abstract void EnsureInvariants();

        protected void Apply(object @event)
        {
            When(@event);
            EnsureInvariants();
            _events.Add(@event);
        }

        public void Load(params object[] events) =>
            events.ToList().ForEach(ev =>
            {
                When(ev);
                Version++;
            });

        public void ClearPendingEvents() => _events.Clear();
    }
}