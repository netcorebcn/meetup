using System;

namespace Meetup.Domain
{
    public class DateTimeRange
    {
        public DateTime Start { get; }
        public DateTime End { get; }

        private DateTimeRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public static DateTimeRange From(DateTime start, DateTime end)
        {
            if (start >= end) throw new ArgumentException("start date must be before end date", nameof(start));
            return new DateTimeRange(start, end);
        }

        public static DateTimeRange From(DateTime start, int durationInHours)
        {
            if (start == default) throw new ArgumentException("start date must be valid", nameof(start));
            return new DateTimeRange(start, start.AddHours(durationInHours));
        }
    }
}