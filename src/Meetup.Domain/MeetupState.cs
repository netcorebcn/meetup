using System;
using System.Collections.Generic;
using System.Linq;
using Meetup.Domain.Events;

namespace Meetup.Domain
{
    public sealed class MeetupState : Enumeration
    {
        private readonly Type[] _raisableEvents;

        private MeetupState(string name, Type[] raisableEvents) : base(name) => 
            _raisableEvents = raisableEvents;

        public bool CanRaiseEvent(Type @eventType) => _raisableEvents.Any(x => x == @eventType);

        public static readonly MeetupState Empty = new MeetupState
        (
            nameof(Empty),
            raisableEvents: new Type[]
            {
                typeof(MeetupRsvpOpenedEvent)
            }
        );

        public static readonly MeetupState Published = new MeetupState
        (
            nameof(Published),
            raisableEvents: new Type[]
            {
                typeof(MeetupCanceledEvent),
                typeof(MeetupRsvpClosedEvent),
                typeof(MeetupRsvpAcceptedEvent),
                typeof(MeetupRsvpDeclinedEvent),
                typeof(MeetupNumberOfSpotsChangedEvent)
            }
        );

        public static readonly MeetupState Closed = new MeetupState
        (
            nameof(Closed),
            raisableEvents: new Type[]
            {
                typeof(MeetupCanceledEvent),
                typeof(MeetupRsvpOpenedEvent),
                typeof(MeetupMemberWentEvent)
            }
        );

        public static readonly MeetupState Cancelled = new MeetupState
        (
            nameof(Cancelled),
            raisableEvents: new Type[]{}
        );
    }
}