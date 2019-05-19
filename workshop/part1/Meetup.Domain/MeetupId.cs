using System;

namespace Meetup.Domain
{
    public class MeetupId
    {
        public Guid Value { get; }

        public MeetupId(Guid id) => Value = id;

        public static MeetupId None => new MeetupId(default(Guid));

        public static MeetupId From(Guid id)
        {
            if (id == default) throw new ArgumentException("Id must be valid", nameof(id));
            return new MeetupId(id);
        }

        public static implicit operator Guid(MeetupId text) => text.Value;
    }
}