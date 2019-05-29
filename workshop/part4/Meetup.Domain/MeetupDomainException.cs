using System;

namespace Meetup.Domain
{
    public class MeetupDomainException : Exception
    {
        public MeetupDomainException(string message) : base(message)
        {
        }
    }
}