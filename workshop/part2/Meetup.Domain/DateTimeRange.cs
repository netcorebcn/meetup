using System;
using System.Collections.Generic;
using System.Globalization;

namespace Meetup.Domain
{
    public class DateTimeRange : ValueObject
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
            return (DateTime.TryParseExact($"{date} {time}", pattern, null, DateTimeStyles.None, out DateTime start))
                ? new DateTimeRange(start, start.AddHours(durationInHours))
                : throw new ArgumentException($"date and time must be in {pattern} format");
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Start;
            yield return End;
        }
    }
}