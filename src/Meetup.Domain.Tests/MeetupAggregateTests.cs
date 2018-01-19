using System;
using Xunit;
using Meetup.Domain;
using System.Linq;

namespace Meetup.Domain.Tests
{
    public class MeetupAggregateTests
    {
        [Fact]
        public void Given_Meetup_When_Publish_Then_PublishedEventRaised()
        {
            var meetup = new MeetupAggregate();
            meetup.Publish();

            var result = meetup.PendingEvents.First();
            Assert.NotNull(result);
            Assert.Equal(typeof(MeetupPublishedEvent), result.GetType());
        }
    }
}
