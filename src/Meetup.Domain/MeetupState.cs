using System;
using System.Collections.Generic;
using System.Linq;
using Meetup.Domain.Events;

namespace Meetup.Domain
{
    public class MeetupState : Enumeration
    {
        private readonly Type[] _raisableEvents;

        private MeetupState(string name, Type[] raisableEvents) : base(name) => 
            _raisableEvents = raisableEvents;

        public bool CanRaiseEvent(Type @eventType) => _raisableEvents.Any(x => x == @eventType);

        public static MeetupState Empty => new MeetupState
        (
            nameof(Empty),
            raisableEvents: new Type[]
            {
                typeof(MeetupRsvpOpenedEvent)
            }
        );

        public static MeetupState Published => new MeetupState
        (
            nameof(Published),
            raisableEvents: new Type[]
            {
                typeof(MeetupCanceledEvent),
                typeof(MeetupRsvpClosedEvent),
                typeof(MeetupRsvpAcceptedEvent),
                typeof(MeetupRsvpDeclinedEvent)
            }
        );

        public static MeetupState Closed => new MeetupState
        (
            nameof(Closed),
            raisableEvents: new Type[]
            {
                typeof(MeetupCanceledEvent),
                typeof(MeetupRsvpOpenedEvent)
            }
        );

        public static MeetupState Cancelled => new MeetupState
        (
            nameof(Cancelled),
            raisableEvents: new Type[]{}
        );
    }
}