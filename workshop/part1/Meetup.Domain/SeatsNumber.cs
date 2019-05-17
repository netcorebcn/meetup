using System;

namespace Meetup.Domain
{
    public class SeatsNumber
    {
        public int Value { get; }

        private SeatsNumber(int number)
        {
            if (number < 1) throw new ArgumentException("Number of seats must be greater than 0", nameof(number));
            Value = number;
        }

        public static SeatsNumber From(int number) => new SeatsNumber(number);
    }
}