using System;

namespace Meetup.Domain
{
    public class Address
    {
        public string Value { get; }

        private Address(string address)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentException("Address must be specified", nameof(address));
            Value = address;
        }

        public static Address From(string address) => new Address(address);
        public static implicit operator string(Address text) => text.Value;
    }
}