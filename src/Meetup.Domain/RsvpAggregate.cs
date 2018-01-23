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

        public static RsvpAggregate Create(Guid meetupId, params object[] events)
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
}