using System;
using System.Collections.Generic;

namespace Meetup.Domain
{
    public class MemberId : ValueObject
    {
        public Guid Value { get; }

        public MemberId(Guid id) => Value = id;

        public static MemberId None => new MemberId(default(Guid));

        public static MemberId From(Guid id)
        {
            if (id == default) throw new ArgumentException("Id must be valid", nameof(id));
            return new MemberId(id);
        }

        public static implicit operator Guid(MemberId text) => text.Value;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}