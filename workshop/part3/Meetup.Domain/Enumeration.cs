
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Meetup.Domain
{
    public class Enumeration : IComparable
    {
        protected Enumeration(string name) => Name = name;

        public string Name { get; }

        public override string ToString() => Name.ToLowerInvariant();

        public override bool Equals(object obj)
        {
            if (!(obj is Enumeration otherValue))
            {
                return false;
            }

            var typeMatches = GetType() == obj.GetType();
            var valueMatches = Name.Equals(otherValue.Name);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode() => Name.GetHashCode();

        public int CompareTo(object other) => string.Compare(Name, ((Enumeration)other).Name, StringComparison.Ordinal);
    }
}