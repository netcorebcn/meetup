using System;

namespace Meetup.Domain
{
    public class MeetupTitle
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
        public static implicit operator string(MeetupTitle text) => text.Value;
    }
}