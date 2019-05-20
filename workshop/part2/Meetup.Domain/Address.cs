using System;
using System.Collections.Generic;

namespace Meetup.Domain
{
    public class Address : ValueObject
    {
        public string Value { get; }

        private Address(string address)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentException("Address must be specified", nameof(address));
            Value = address;
        }
        public static Address None => From("none");

        public static Address From(string address) => new Address(address);

        public static implicit operator string(Address text) => text.Value;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}