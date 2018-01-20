using System;
using Xunit;
using Meetup.Domain;
using System.Linq;
using Meetup.Domain.Events;

namespace Meetup.Domain.Tests
{
    public class MeetupAggregateTests
    {
        [Fact]
        public void Given_Meetup_When_Publish_Then_RSVPOpened()
        {
            var meetup = new MeetupAggregate();
            meetup.Publish();

            var result = meetup.PendingEvents.First();
            Assert.NotNull(result);
            Assert.Equal(typeof(MeetupRsvpOpenedEvent), result.GetType());
        }
        
        [Fact]
        public void Given_Published_Meetup_When_Cancel_Then_Canceled()
        {
            var meetup = new MeetupAggregate();
            meetup.Cancel();

            var result = meetup.PendingEvents.First();
            Assert.NotNull(result);
            Assert.Equal(typeof(MeetupCanceledEvent), result.GetType());
        }
    }
}
