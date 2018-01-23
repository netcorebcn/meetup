using System;
using System.Collections.Generic;
using System.Linq;
using Meetup.Domain.Events;

namespace Meetup.Domain
{
    public class RsvpAggregate
    {
        private int _numberOfSpots;

        public List<Guid> MembersGoing { get; } = new List<Guid>();

        public List<Guid> MembersNotGoing { get; } = new List<Guid>();

        public List<Guid> MembersWaiting { get; } = new List<Guid>();

        private RsvpAggregate()
        {
        }

        public RsvpAggregate(List<Guid> membersGoing, List<Guid> membersWaiting, int numberOfSpots)
        {
            MembersGoing = membersGoing;
            MembersWaiting = membersWaiting;
            _numberOfSpots = numberOfSpots;
        }

        public static RsvpAggregate Create(params object[] events)
        {
            return events.Aggregate(new RsvpAggregate(), Reduce);
        }

        public static RsvpAggregate Reduce(RsvpAggregate state, object @event)
        {
            switch (@event)
            {
                case MeetupRsvpOpenedEvent rsvpOpened:
                    state._numberOfSpots = rsvpOpened.NumberOfSpots;
                    break;

                case MeetupRsvpAcceptedEvent rsvpAccepted:
                    UpdateMembersGoingList(state, rsvpAccepted.MemberId);
                    break;

                case MeetupRsvpDeclinedEvent rsvpDeclined:
                    UpdateWaitingList(state, rsvpDeclined.MemberId);
                    break;

                case MeetupNumberOfSpotsChangedEvent spotsChanged:
                    state = Reduce(state, spotsChanged.NumberOfSpots);
                    break;
            }
            return state;
        }

        public static RsvpAggregate Reduce(RsvpAggregate state, int newNumberOfSpots)
        {
            var diff = newNumberOfSpots - state._numberOfSpots; 

            if (diff > 0)
            {
                var membersWaiting = state.MembersWaiting.Take(diff);
                var membersWaitingCount = state.MembersWaiting.Count;
                state.MembersWaiting.RemoveRange(membersWaitingCount - diff, diff); 
                state.MembersGoing.AddRange(membersWaiting);
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

        private static void UpdateWaitingList(RsvpAggregate state, Guid memberId)
        {
            state.MembersNotGoing.Add(memberId);
            state.MembersGoing.Remove(memberId);
            state.MembersWaiting.Remove(memberId);
            
            if (state._numberOfSpots > state.MembersGoing.Count)
            {
                var firstMemberWaiting = state.MembersWaiting.FirstOrDefault();
                if (firstMemberWaiting != null)
                {
                    state.MembersGoing.Add(firstMemberWaiting);
                    state.MembersWaiting.Remove(firstMemberWaiting);
                }
            }
        }
        
        private static void UpdateMembersGoingList(RsvpAggregate state, Guid memberId)
        {
            if (!state.MembersGoing.Contains(memberId))
            {
                if (state._numberOfSpots > state.MembersGoing.Count)
                {
                    state.MembersGoing.Add(memberId);
                } 
                else
                {
                    state.MembersWaiting.Add(memberId);
                }
            }
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