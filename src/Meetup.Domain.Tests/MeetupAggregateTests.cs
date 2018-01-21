using System;
using Xunit;
using Meetup.Domain;
using System.Linq;
using Meetup.Domain.Events;
using System.Collections.Generic;

namespace Meetup.Domain.Tests
{
    public class MeetupAggregateTests
    {
        [Fact]
        public void Given_NewMeetup_When_Publish_Then_RSVPOpened()
        {
            var meetup = new MeetupAggregate();
            meetup.Publish();

            meetup.GetPendingEvents().AssertLastEventOfType<MeetupRsvpOpenedEvent>().WithTotalCount(1);
        }
        
        [Fact]
        public void Given_Published_Meetup_When_Cancel_Then_Canceled()
        {
            var meetup = new MeetupAggregate();
            meetup.Publish();
            meetup.Cancel();

            meetup.GetPendingEvents().AssertLastEventOfType<MeetupCanceledEvent>().WithTotalCount(2);
        }

        [Fact]
        public void Given_Canceled_Meetup_When_Publish_Then_Still_Canceled()
        {
            var meetup = new MeetupAggregate();
            meetup.Publish();
            meetup.Cancel();
            meetup.Publish();

            meetup.GetPendingEvents().AssertLastEventOfType<MeetupCanceledEvent>().WithTotalCount(2);
        }

        [Fact]
        public void Given_OpenedMeetup_When_AcceptRsvp_Then_RsvpAccepted()
        {
            var meetup = new MeetupAggregate();
            meetup.Publish();
            meetup.AcceptRsvp();

            meetup.GetPendingEvents().AssertLastEventOfType<MeetupRsvpAcceptedEvent>().WithTotalCount(2);
        }

        [Fact]
        public void Given_ClosedMeetup_When_AcceptRsvp_Then_NotAccepted()
        {
            var meetup = new MeetupAggregate();
            meetup.Publish();
            meetup.Close();
            meetup.AcceptRsvp();
            
            meetup.GetPendingEvents().AssertLastEventOfType<MeetupRsvpClosedEvent>().WithTotalCount(2);
        }
    }

    public static class AggregateTestsExtensions
    {
        public static IEnumerable<object> AssertLastEventOfType<T>(this IEnumerable<object> events, int eventsCount = 1) 
        {
            var lastEvent = events.Last();
            Assert.NotNull(lastEvent);
            Assert.Equal(typeof(T), lastEvent.GetType());
            return events;
        }

        public static IEnumerable<object> WithTotalCount(this IEnumerable<object> events, int totalCount = 1)
        {
            Assert.Equal(events.Count(), totalCount);
            return events;
        }
    }
}