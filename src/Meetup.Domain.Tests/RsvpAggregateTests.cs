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

            var aggregate = RsvpAggregate.Create(meetupId, new MeetupRsvpOpenedEvent(meetupId, numberOfSpots: 2));
            aggregate.MembersGoing.AssertEqual();
            aggregate.MembersWaiting.AssertEqual();
            aggregate.MembersNotGoing.AssertEqual();

            aggregate.Reduce(new MeetupRsvpAcceptedEvent(meetupId, bill));
            aggregate.MembersGoing.AssertEqual(bill);
            aggregate.MembersWaiting.AssertEqual();
            aggregate.MembersNotGoing.AssertEqual();

            aggregate.Reduce(new MeetupRsvpAcceptedEvent(meetupId, joe));
            aggregate.MembersGoing.AssertEqual(bill, joe);
            aggregate.MembersWaiting.AssertEqual();
            aggregate.MembersNotGoing.AssertEqual();

            aggregate.Reduce(new MeetupRsvpAcceptedEvent(meetupId, susan));
            aggregate.MembersGoing.AssertEqual(bill, joe);
            aggregate.MembersWaiting.AssertEqual(susan);
            aggregate.MembersNotGoing.AssertEqual();
                
            aggregate.Reduce(new MeetupRsvpAcceptedEvent(meetupId, carla));
            aggregate.MembersGoing.AssertEqual(bill, joe);
            aggregate.MembersWaiting.AssertEqual(susan, carla);
            aggregate.MembersNotGoing.AssertEqual();

            aggregate.Reduce(new MeetupRsvpDeclinedEvent(meetupId, bill));
            aggregate.MembersGoing.AssertEqual(joe, susan);
            aggregate.MembersWaiting.AssertEqual(carla);
            aggregate.MembersNotGoing.AssertEqual(bill);
        }

        [Fact]
        public void Test()
        {
            var bill = Guid.NewGuid();
            var joe = Guid.NewGuid();
            var susan = Guid.NewGuid();
            var carla = Guid.NewGuid();

            var memberList = new List<Guid>() { bill, joe, susan };
            var waitingList = new List<Guid>() { carla };
            var newNumberOfSports = 2;

            var aggregate = new RsvpAggregate(memberList, waitingList, numberOfSpots: 3);
            aggregate= RsvpAggregate.Reduce(aggregate, newNumberOfSports);

            aggregate.MembersGoing.AssertEqual(bill, joe);
            aggregate.MembersWaiting.AssertEqual(susan, carla);
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