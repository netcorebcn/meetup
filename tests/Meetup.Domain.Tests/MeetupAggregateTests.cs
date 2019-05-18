using System;
using Xunit;
using Meetup.Domain;
using System.Linq;
using System.Collections.Generic;

namespace Meetup.Domain.Tests
{
    public class MeetupAggregateTests
    {
        [Fact]
        public void Given_NewMeetup_When_Publish_Then_RSVPOpened() =>
            ExecuteCommand(meetup =>
                meetup.Publish(numberOfSpots: 4))
                .GetPendingEvents()
                .AssertLastEventOfType<Events.MeetupRsvpOpened>()
                .WithTotalCount(1);

        [Fact]
        public void Given_Published_Meetup_When_Cancel_Then_Canceled() =>
            ExecuteCommand(meetup =>
            {
                meetup.Publish(numberOfSpots: 4);
                meetup.Cancel();
            })
            .GetPendingEvents()
            .AssertLastEventOfType<Events.MeetupCanceled>()
            .WithTotalCount(2);

        [Fact]
        public void Given_Canceled_Meetup_When_Publish_Then_Still_Canceled() =>
            ExecuteCommand(meetup =>
            {
                var meetupId = Guid.NewGuid();
                meetup.Publish(numberOfSpots: 4);
                meetup.Cancel();
                meetup.Publish(numberOfSpots: 4);
            })
            .GetPendingEvents()
            .AssertLastEventOfType<Events.MeetupCanceled>()
            .WithTotalCount(2);

        [Fact]
        public void Given_OpenedMeetup_When_AcceptingRsvp_Then_RsvpAccepted() =>
            ExecuteCommand(meetup =>
            {
                meetup.Publish(numberOfSpots: 4);
                meetup.DeclineRsvp(Guid.NewGuid());
                meetup.AcceptRsvp(Guid.NewGuid());
            })
            .GetPendingEvents()
            .AssertLastEventOfType<Events.MeetupRsvpAccepted>()
            .WithTotalCount(3);

        [Fact]
        public void Given_ClosedMeetup_When_AcceptRsvp_Then_NotAccepted() =>
            ExecuteCommand(meetup =>
            {
                meetup.Publish(numberOfSpots: 4);
                meetup.Close();
                meetup.AcceptRsvp(Guid.NewGuid());
                meetup.DeclineRsvp(Guid.NewGuid());
            })
            .GetPendingEvents()
            .AssertLastEventOfType<Events.MeetupRsvpClosed>()
            .WithTotalCount(2);

        [Fact]
        public void Given_ClosedMeetup_When_TakeAttendance_Then_MemberWent() =>
            ExecuteCommand(meetup =>
            {
                meetup.Publish(numberOfSpots: 4);
                meetup.Close();
                meetup.TakeAttendance(Guid.NewGuid());
            })
            .GetPendingEvents()
            .AssertLastEventOfType<Events.MeetupMemberWent>()
            .WithTotalCount(3);

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