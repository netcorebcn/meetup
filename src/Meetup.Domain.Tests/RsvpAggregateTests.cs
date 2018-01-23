using System;
using System.Collections.Generic;
using System.Linq;
using Meetup.Domain.Events;
using Xunit;

namespace Meetup.Domain.Tests
{
    public class RsvpAggregateTests
    {
        [Fact]
        public void Given_MeetupEvents_When_Aggregate_Then_RsvpListCreated()
        {
            var meetupId = Guid.NewGuid();
            var bill = Guid.NewGuid();
            var joe = Guid.NewGuid();
            var susan = Guid.NewGuid();
            var carla = Guid.NewGuid();

            var rsvpAggregate = RsvpAggregate.Create(
                meetupId, 
                new MeetupRsvpOpenedEvent(meetupId, numberOfSpots: 2));

            rsvpAggregate.MembersGoing.AssertEqual();
            rsvpAggregate.MembersWaiting.AssertEqual();
            rsvpAggregate.MembersNotGoing.AssertEqual();

            rsvpAggregate = RsvpAggregate.Reduce(
                rsvpAggregate, 
                new MeetupRsvpAcceptedEvent(meetupId, bill));

            rsvpAggregate.MembersGoing.AssertEqual(bill);
            rsvpAggregate.MembersWaiting.AssertEqual();
            rsvpAggregate.MembersNotGoing.AssertEqual();

            rsvpAggregate = RsvpAggregate.Reduce(
                rsvpAggregate, 
                new MeetupRsvpAcceptedEvent(meetupId, joe));

            rsvpAggregate.MembersGoing.AssertEqual(bill, joe);
            rsvpAggregate.MembersWaiting.AssertEqual();
            rsvpAggregate.MembersNotGoing.AssertEqual();

            rsvpAggregate = RsvpAggregate.Reduce(
                rsvpAggregate, 
                new MeetupRsvpAcceptedEvent(meetupId, susan));

            rsvpAggregate.MembersGoing.AssertEqual(bill, joe);
            rsvpAggregate.MembersWaiting.AssertEqual(susan);
            rsvpAggregate.MembersNotGoing.AssertEqual();
                
            rsvpAggregate = RsvpAggregate.Reduce(
                rsvpAggregate, 
                new MeetupRsvpAcceptedEvent(meetupId, carla));

            rsvpAggregate.MembersGoing.AssertEqual(bill, joe);
            rsvpAggregate.MembersWaiting.AssertEqual(susan, carla);
            rsvpAggregate.MembersNotGoing.AssertEqual();

            rsvpAggregate = RsvpAggregate.Reduce(
                rsvpAggregate, 
                new MeetupRsvpDeclinedEvent(meetupId, bill));

            rsvpAggregate.MembersGoing.AssertEqual(joe, susan);
            rsvpAggregate.MembersWaiting.AssertEqual(carla);
            rsvpAggregate.MembersNotGoing.AssertEqual(bill);
        }
    }
    public static class RsvpAggregateTestsExtensions
    {
        public static void AssertEqual(this List<Guid> list, params Guid[] guids)
        {
            Assert.Equal(guids.ToList(), list);
        }
    }
}