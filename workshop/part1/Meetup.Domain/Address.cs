using System;

namespace Meetup.Domain
{
    public class Address : IEquatable<Address>
    {
        public string Value { get; }
        public static Address None => From("none");

        private Address(string address)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentException("Address must be specified", nameof(address));
            Value = address;
        }

        public static Address From(string address) => new Address(address);

        public bool Equals(Address other) => this.Value == other.Value;

        public override bool Equals(object other) => this.Value == ((Address)other).Value;

        public override int GetHashCode() => Value.GetHashCode();
        public static bool operator ==(Address left, Address right) => Equals(left, right);
        public static bool operator !=(Address left, Address right) => !Equals(left, right);

        public static implicit operator string(Address text) => text.Value;
    }
}