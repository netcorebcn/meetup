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
        public void Given_NumberOfSpots_When_Build_Then_RsvpAggregateCreated()
        {
            var aggregate = RsvpAggregate.WithNumberOfSpots(1);
            Assert.Equal(1, aggregate.NumberOfSpots);
            aggregate.MembersGoing.AssertEqual();
            aggregate.MembersNotGoing.AssertEqual();
            aggregate.MembersWaiting.AssertEqual();
        }

        [Fact]
        public void Given_NumberOfSpots_And_Members_When_Build_Then_RsvpAggregateCreated()
        {
            Guid bill = Guid.NewGuid(), joe = Guid.NewGuid(), susan = Guid.NewGuid(), carla = Guid.NewGuid();
            
            var aggregate = RsvpAggregate
                .WithNumberOfSpots(4)
                .WithMembersGoing(bill, joe, susan, carla);

            Assert.Equal(4, aggregate.NumberOfSpots);
            aggregate.MembersGoing.AssertEqual(bill, joe, susan, carla);
            aggregate.MembersNotGoing.AssertEqual();
            aggregate.MembersWaiting.AssertEqual();
        }

        [Fact]
        public void Given_NumberOfSpots_And_Members_And_Waiting_When_Build_Then_RsvpAggregateCreated()
        {
            Guid bill = Guid.NewGuid(), joe = Guid.NewGuid(), susan = Guid.NewGuid(), carla = Guid.NewGuid();
            
            var aggregate = RsvpAggregate
                .WithNumberOfSpots(2)
                .WithMembersGoing(bill, joe)
                .WithMembersWaiting(carla, susan);

            Assert.Equal(2, aggregate.NumberOfSpots);
            aggregate.MembersGoing.AssertEqual(bill, joe);
            aggregate.MembersWaiting.AssertEqual(carla, susan);
            aggregate.MembersNotGoing.AssertEqual();
        }

        [Fact]
        public void Given_NumberOfSpots_And_Members_And_Waiting_And_NotGoing_When_Build_Then_RsvpAggregateCreated()
        {
            Guid bill = Guid.NewGuid(), joe = Guid.NewGuid(), susan = Guid.NewGuid(), carla = Guid.NewGuid();
            
            var aggregate = RsvpAggregate
                .WithNumberOfSpots(2)
                .WithMembersGoing(bill, joe)
                .WithMembersWaiting(carla)
                .WithMembersNotGoing(susan);

            Assert.Equal(2, aggregate.NumberOfSpots);
            aggregate.MembersGoing.AssertEqual(bill, joe);
            aggregate.MembersWaiting.AssertEqual(carla);
            aggregate.MembersNotGoing.AssertEqual(susan);
        }
            
        [Fact]
        public void Given_MeetupEvents_When_Aggregate_Then_RsvpListCreated()
        {
            Guid meetupId = Guid.NewGuid(), bill = Guid.NewGuid(), joe = Guid.NewGuid(), susan = Guid.NewGuid(), carla = Guid.NewGuid();

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

            aggregate.Reduce(new MeetupNumberOfSpotsChangedEvent(meetupId, 3));
            aggregate.MembersGoing.AssertEqual(joe, susan, carla);
            aggregate.MembersWaiting.AssertEqual();
            aggregate.MembersNotGoing.AssertEqual(bill);
        }
        

        [Fact]
        public void Given_RsvpAggregate_When_Reduce_With_Same_NumberOfSpots_Then_ExpectedRsvpAggregate()
        {
            Guid meetupId = Guid.NewGuid(), bill = Guid.NewGuid(), joe = Guid.NewGuid(), susan = Guid.NewGuid(), carla = Guid.NewGuid();

            var aggregate = RsvpAggregate
                .WithNumberOfSpots(3)
                .WithMembersGoing(bill, joe, susan)
                .WithMembersWaiting(carla);

            aggregate= aggregate.Reduce(new MeetupNumberOfSpotsChangedEvent(meetupId, 3));

            aggregate.MembersGoing.AssertEqual(bill, joe, susan);
            aggregate.MembersWaiting.AssertEqual(carla);
            aggregate.MembersNotGoing.AssertEqual();
        }

        [Fact]
        public void Given_RsvpAggregate_When_Reduce_With_More_NumberOfSpots_Then_ExpectedRsvpAggregate()
        {
            Guid meetupId = Guid.NewGuid(), bill = Guid.NewGuid(), joe = Guid.NewGuid(), susan = Guid.NewGuid(), carla = Guid.NewGuid();

            var aggregate = RsvpAggregate
                .WithNumberOfSpots(3)
                .WithMembersGoing(bill, joe, susan)
                .WithMembersWaiting(carla);

            aggregate= aggregate.Reduce(new MeetupNumberOfSpotsChangedEvent(meetupId, 4));

            aggregate.MembersGoing.AssertEqual(bill, joe, susan, carla);
            aggregate.MembersWaiting.AssertEqual();
            aggregate.MembersNotGoing.AssertEqual();
        }
        
        [Fact]
        public void Given_RsvpAggregate_When_Reduce_With_Less_NumberOfSpots_Then_ExpectedRsvpAggregate()
        {
            Guid meetupId = Guid.NewGuid(), bill = Guid.NewGuid(), joe = Guid.NewGuid(), susan = Guid.NewGuid(), carla = Guid.NewGuid();

            var aggregate = RsvpAggregate
                .WithNumberOfSpots(3)
                .WithMembersGoing(bill, joe, susan)
                .WithMembersWaiting(carla);

            aggregate= aggregate.Reduce(new MeetupNumberOfSpotsChangedEvent(meetupId, 2));

            aggregate.MembersGoing.AssertEqual(bill, joe);
            aggregate.MembersWaiting.AssertEqual(susan, carla);
            aggregate.MembersNotGoing.AssertEqual();
        }

        [Fact]
        public void Given_MeetupEvents_When_Reduce_RsvpDeclined_Then_ExpectedRsvpAggregate()
        {
            Guid meetupId = Guid.NewGuid(), bill = Guid.NewGuid(), joe = Guid.NewGuid(), susan = Guid.NewGuid(), carla = Guid.NewGuid();

            var aggregate = RsvpAggregate
                .WithNumberOfSpots(4)
                .WithMembersGoing(bill, joe, susan, carla);
                
            aggregate= aggregate.Reduce(new MeetupRsvpDeclinedEvent(meetupId, bill));

            aggregate.MembersGoing.AssertEqual(joe, susan, carla);
            aggregate.MembersNotGoing.AssertEqual(bill);
            aggregate.MembersWaiting.AssertEqual();
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