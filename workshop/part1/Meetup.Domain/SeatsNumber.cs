using System;

namespace Meetup.Domain
{
    public class SeatsNumber : IEquatable<SeatsNumber>
    {
        public int Value { get; }

        private SeatsNumber(int number)
        {
            if (number < 1) throw new ArgumentException("Number of seats must be greater than 0", nameof(number));
            Value = number;
        }

        public static SeatsNumber None => From(1);
        public static SeatsNumber From(int number) => new SeatsNumber(number);

        public bool Equals(SeatsNumber other) => this.Value == other.Value;

        public override bool Equals(object other) => this.Value == ((SeatsNumber)other).Value;

        public override int GetHashCode() => Value.GetHashCode();
        public static bool operator ==(SeatsNumber left, SeatsNumber right) => Equals(left, right);
        public static bool operator !=(SeatsNumber left, SeatsNumber right) => !Equals(left, right);
        public static implicit operator int(SeatsNumber number) => number.Value;
    }
}