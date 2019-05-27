using System;
using System.Linq;

namespace Meetup.Domain
{
    public class MeetupState : Enumeration
    {
        public static readonly MeetupState Created = new MeetupState(nameof(Created),
            typeof(Events.MeetupLocationUpdated),
            typeof(Events.MeetupTimeUpdated),
            typeof(Events.MeetupTitleUpdated),
            typeof(Events.MeetupNumberOfSeatsUpdated),
            typeof(Events.MeetupPublished)
        );

        public static readonly MeetupState Published = new MeetupState(nameof(Published));
        public static readonly MeetupState Canceled = new MeetupState(nameof(Canceled));
        public static readonly MeetupState Closed = new MeetupState(nameof(Closed));

        public bool CanRaiseEvent(Type eventType) => _events.Any(type => type == eventType);
        public bool CanRaiseEvent<T>() => CanRaiseEvent(typeof(T));

        private readonly Type[] _events;
        private MeetupState(string name, params Type[] events) : base(name) => _events = events;
    }
}