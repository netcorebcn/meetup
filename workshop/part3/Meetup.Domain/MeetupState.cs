using System;

namespace Meetup.Domain
{
    public class MeetupState : Enumeration
    {
        public static readonly MeetupState Created = new MeetupState(nameof(Created));
        public static readonly MeetupState Published = new MeetupState(nameof(Published));
        public static readonly MeetupState Canceled = new MeetupState(nameof(Canceled));
        public static readonly MeetupState Closed = new MeetupState(nameof(Closed));

        private MeetupState(string name) : base(name) { }
    }
}