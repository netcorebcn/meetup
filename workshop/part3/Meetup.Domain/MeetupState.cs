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
            typeof(Events.MeetupPublished),
            typeof(Events.MeetupCreated)
        );

        public static readonly MeetupState Published = new MeetupState(nameof(Published),
            typeof(Events.MeetupLocationUpdated),
            typeof(Events.MeetupTimeUpdated),
            typeof(Events.MeetupTitleUpdated),
            typeof(Events.MeetupNumberOfSeatsUpdated),
            typeof(Events.MeetupPublished),
            typeof(Events.MeetupCanceled),
            typeof(Events.MeetupClosed),
            typeof(Events.RSVPAccepted),
            typeof(Events.RSVPRejected)
        );

        public static readonly MeetupState Canceled = new MeetupState(nameof(Canceled),
            typeof(Events.MeetupCanceled));

        public static readonly MeetupState Closed = new MeetupState(nameof(Closed),
            typeof(Events.MeetupClosed));

        public bool CanRaiseEvent(Type eventType) => _events.Any(type => type == eventType);
        public bool CanRaiseEvent<T>() => CanRaiseEvent(typeof(T));
        public void EnsureCanRaiseEvent(Type eventType)
        {
            if (!CanRaiseEvent(eventType))
            {
                throw new MeetupDomainException($"State {Name}, Can not raise {eventType.Name}");
            }
        }

        private readonly Type[] _events;
        private MeetupState(string name, params Type[] events) : base(name) => _events = events;
    }
}