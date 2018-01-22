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
        public void Given_NewMeetup_When_Publish_Then_RSVPOpened() => 
            ExecuteCommand(meetup => meetup.Publish())
                .GetPendingEvents().AssertLastEventOfType<MeetupRsvpOpenedEvent>()
                .WithTotalCount(1);

        [Fact]
        public void Given_Published_Meetup_When_Cancel_Then_Canceled() =>
            ExecuteCommand(meetup => 
            {
                meetup.Publish();
                meetup.Cancel();
            })
            .GetPendingEvents().AssertLastEventOfType<MeetupCanceledEvent>()
            .WithTotalCount(2);

        [Fact]
        public void Given_Canceled_Meetup_When_Publish_Then_Still_Canceled() =>
            ExecuteCommand(meetup => 
            {
                meetup.Publish();
                meetup.Cancel();
                meetup.Publish();
            })
            .GetPendingEvents().AssertLastEventOfType<MeetupCanceledEvent>()
            .WithTotalCount(2);

        [Fact]
        public void Given_OpenedMeetup_When_AcceptRsvp_Then_RsvpAccepted() =>
            ExecuteCommand(meetup =>
            {
                meetup.Publish();
                meetup.AcceptRsvp(Guid.NewGuid());
            })
            .GetPendingEvents().AssertLastEventOfType<MeetupRsvpAcceptedEvent>()
            .WithTotalCount(2);

        [Fact]
        public void Given_ClosedMeetup_When_AcceptRsvp_Then_NotAccepted() =>
            ExecuteCommand(meetup =>
            {
                meetup.Publish();
                meetup.Close();
                meetup.AcceptRsvp(Guid.NewGuid());
            })
            .GetPendingEvents().AssertLastEventOfType<MeetupRsvpClosedEvent>().WithTotalCount(2);

        private MeetupAggregate ExecuteCommand(Action<MeetupAggregate> command)
        {
            var meetupId = Guid.NewGuid();
            var meetup = new MeetupAggregate(meetupId);
            command(meetup);
            return meetup;
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