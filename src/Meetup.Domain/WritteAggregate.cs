using System;
using System.Collections.Generic;
using System.Linq;

namespace Meetup.Domain
{
    public class WriteAggregate
    {
        public List<Guid> RSPV;
        public int NumberOfSpots;

        public MeetupState State { get; private set; } = MeetupState.Empty;

        public List<object> PendingEvents = new List<object>();

        public static WriteAggregate Reduce(WriteAggregate aggregate, object @event)
        {
            switch (@event)
            {
                case Events.MeetupRsvpOpened created:
                    aggregate.State = MeetupState.Published;
                    aggregate.NumberOfSpots = created.NumberOfSpots;
                    break;
                case Events.MeetupRsvpAccepted accepted:
                    aggregate.RSPV.Add(accepted.MemberId);
                    break;
                case Events.MeetupRsvpDeclined declined:
                    aggregate.RSPV.Remove(declined.MemberId);
                    break;
            }

            return aggregate;
        }

        public static WriteAggregate Create(List<object> events) => events.Aggregate(new WriteAggregate(), Reduce);

        public (bool, string) AcceptRsvp(AcceptRsvpCommand accept)
        {
            if (RSPV.Count >= NumberOfSpots)
            {
                return (false, "This meetup is full");
            }

            return TryApplyEvent(accept);
        }

        public (bool, string) TryApplyEvent(object @event)
        {
            if (State.CanRaiseEvent(@event.GetType()))
            {
                PendingEvents.Add(@event);
                return (true, null);
            }

            return (false, "Current state doesn't allow to apply this event");
        }
    }
}