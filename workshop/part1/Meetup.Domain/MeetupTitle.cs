using System;

namespace Meetup.Domain
{
    public class MeetupTitle : IEquatable<MeetupTitle>
    {
        public string Value { get; }

        private MeetupTitle(string title)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentException("Title must be specified", nameof(title));
            if (title.Length > 50) throw new ArgumentException("Title must be less than 25 characters", nameof(title));
            Value = title;
        }
        public static MeetupTitle None => From(" ");
        public static MeetupTitle From(string title) => new MeetupTitle(title);

        public bool Equals(MeetupTitle other) => Value == other.Value;

        public override bool Equals(object other) => this.Value == ((MeetupTitle)other).Value;

        public override int GetHashCode() => Value.GetHashCode();
        public static bool operator ==(MeetupTitle left, MeetupTitle right) => Equals(left, right);
        public static bool operator !=(MeetupTitle left, MeetupTitle right) => !Equals(left, right);
        public static implicit operator string(MeetupTitle text) => text.Value;
    }
}