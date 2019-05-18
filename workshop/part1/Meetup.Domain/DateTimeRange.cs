using System;
using System.Globalization;

namespace Meetup.Domain
{
    public class DateTimeRange : IEquatable<DateTimeRange>
    {
        public DateTime Start { get; }
        public DateTime End { get; }

        private DateTimeRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public static DateTimeRange None => From(DateTime.MinValue.AddHours(1), DateTime.MaxValue);

        public static DateTimeRange From(DateTime start, DateTime end)
        {
            if (start == default) throw new ArgumentException("start date must be valid", nameof(start));
            if (start >= end) throw new ArgumentException("start date must be before end date", nameof(start));

            return new DateTimeRange(start, end);
        }

        public static DateTimeRange From(string date, string time, int durationInHours)
        {
            var pattern = "yyyy-MM-dd HH:mm";
            if (DateTime.TryParseExact($"{date} {time}", pattern, null,
                                   DateTimeStyles.None, out DateTime start))
            {
                return new DateTimeRange(start, start.AddHours(durationInHours));
            }

            throw new ArgumentException($"date and time must be in {pattern} format");
        }

        public bool Equals(DateTimeRange other) => Start == other.Start && End == other.End;

        public override bool Equals(object other) => other is DateTimeRange otherTime ? Equals(otherTime) : false;

        public override int GetHashCode() => Start.GetHashCode() + End.GetHashCode();
        public static bool operator ==(DateTimeRange left, DateTimeRange right) => Equals(left, right);
        public static bool operator !=(DateTimeRange left, DateTimeRange right) => !Equals(left, right);
    }
}