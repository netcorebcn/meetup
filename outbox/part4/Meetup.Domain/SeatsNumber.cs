using System;
using System.Collections.Generic;

namespace Meetup.Domain
{
    public class SeatsNumber : ValueObject
    {
        public int Value { get; }

        private SeatsNumber(int number)
        {
            if (number < 1) throw new ArgumentException("Number of seats must be greater than 0", nameof(number));
            Value = number;
        }

        public static SeatsNumber None => From(1);
        public static SeatsNumber From(int number) => new SeatsNumber(number);
        public static implicit operator int(SeatsNumber number) => number.Value;
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}