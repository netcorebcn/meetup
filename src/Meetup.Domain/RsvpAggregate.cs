using System;
using System.Collections.Generic;
using System.Linq;
using Meetup.Domain.Events;

namespace Meetup.Domain
{
    public class RsvpAggregate
    {
        public int NumberOfSpots { get; private set; }

        public List<Guid> MembersGoing { get; } = new List<Guid>();

        public List<Guid> MembersNotGoing { get; } = new List<Guid>();

        public List<Guid> MembersWaiting { get; } = new List<Guid>();

        private RsvpAggregate(List<Guid> membersGoing = null, 
            List<Guid> membersNotGoing = null, 
            List<Guid> membersWaiting = null, 
            int numberOfSpots = 0)
        {
            MembersGoing = membersGoing ?? new List<Guid>();
            MembersNotGoing = membersNotGoing ?? new List<Guid>();
            MembersWaiting = membersWaiting ?? new List<Guid>();
            NumberOfSpots = numberOfSpots;
        }

        public static RsvpAggregate WithNumberOfSpots(int numberOfSpots) => 
            new RsvpAggregate(numberOfSpots: numberOfSpots);

        public RsvpAggregate WithMembersGoing(params Guid[] membersGoing) => 
            new RsvpAggregate(membersGoing.ToList(), this.MembersNotGoing, this.MembersWaiting, this.NumberOfSpots);

        public RsvpAggregate WithMembersNotGoing(params Guid[] membersNotGoing) => 
            new RsvpAggregate(this.MembersGoing, membersNotGoing.ToList(), this.MembersWaiting, this.NumberOfSpots);

        public RsvpAggregate WithMembersWaiting(params Guid[] membersWaiting) => 
            new RsvpAggregate(this.MembersGoing, this.MembersNotGoing, membersWaiting.ToList(), this.NumberOfSpots);

        public static RsvpAggregate Create(params object[] events) => 
            events.Aggregate(new RsvpAggregate(), Reduce);

        public static RsvpAggregate Reduce(RsvpAggregate state, object @event)
        {
            switch (@event)
            {
                case MeetupRsvpOpenedEvent rsvpOpened:
                    state.NumberOfSpots = rsvpOpened.NumberOfSpots;
                    break;

                case MeetupRsvpAcceptedEvent rsvpAccepted:
                    state = Reduce(state, rsvpAccepted);
                    break;

                case MeetupRsvpDeclinedEvent rsvpDeclined:
                    state = Reduce(state, rsvpDeclined);
                    break;

                case MeetupNumberOfSpotsChangedEvent spotsChanged:
                    state = Reduce(state, spotsChanged);
                    break;
            }
            return state;
        }

        private static RsvpAggregate Reduce(RsvpAggregate state, MeetupNumberOfSpotsChangedEvent @event)
        {
            var diff = @event.NumberOfSpots - state.NumberOfSpots; 

            if (diff > 0)
            {
                var membersWaiting = state.MembersWaiting.Take(diff);
                state.MembersGoing.AddRange(membersWaiting);

                var membersWaitingCount = state.MembersWaiting.Count;
                state.MembersWaiting.RemoveRange(membersWaitingCount - diff, diff); 
            }
            else
            {
                var takeLast = Math.Abs(diff);
                var membersGoing = state.MembersGoing.TakeLast(takeLast);
                state.MembersWaiting.InsertRange(0, membersGoing);
                
                var membersGoingCount = state.MembersGoing.Count;
                state.MembersGoing.RemoveRange(membersGoingCount - takeLast, takeLast);
            }

            return state;
        }

        private static bool AvailableSpots(RsvpAggregate state) =>
            state.NumberOfSpots > state.MembersGoing.Count;

        private static RsvpAggregate Reduce(RsvpAggregate state, MeetupRsvpDeclinedEvent @event)
        {
            var memberId = @event.MemberId;
            state.MembersNotGoing.Add(memberId);
            state.MembersGoing.Remove(memberId);
            state.MembersWaiting.Remove(memberId);
            
            if (AvailableSpots(state) && state.MembersWaiting.Any())
            {
                var firstMemberWaiting = state.MembersWaiting.First();
                state.MembersGoing.Add(firstMemberWaiting);
                state.MembersWaiting.Remove(firstMemberWaiting);
            }
            
            return state;
        }
        
        private static RsvpAggregate Reduce(RsvpAggregate state, MeetupRsvpAcceptedEvent @event)
        {
            var memberId = @event.MemberId;
            if (!state.MembersGoing.Contains(memberId))
            {
                if (AvailableSpots(state))
                {
                    state.MembersGoing.Add(memberId);
                } 
                else
                {
                    state.MembersWaiting.Add(memberId);
                }
            }
            return state;
        }
    }

    public static class RsvpAggregateExtensions
    {
        public static RsvpAggregate Reduce(this RsvpAggregate aggregate, object @event) =>
            RsvpAggregate.Reduce(aggregate, @event);

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N) => 
            source.Skip(Math.Max(0, source.Count() - N));
    }
}