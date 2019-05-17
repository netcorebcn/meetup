using System;
using Xunit;

namespace Meetup.Domain.Tests
{
    public class MeetupStateTests
    {
        [Theory]
        [InlineData(typeof(Events.MeetupCanceled))]
        [InlineData(typeof(Events.MeetupRsvpClosed))]
        [InlineData(typeof(Events.MeetupRsvpAccepted))]
        [InlineData(typeof(Events.MeetupRsvpDeclined))]
        [InlineData(typeof(Events.MeetupNumberOfSpotsChanged))]
        public void Given_MeetupState_When_Published_Then_CanRaiseEvent(Type eventType) =>
            Assert.True(MeetupState.Published.CanRaiseEvent(eventType));

        [Theory]
        [InlineData(typeof(Events.MeetupRsvpOpened))]
        public void Given_MeetupState_When_Published_Then_CanNotRaiseEvent(Type eventType) =>
            Assert.False(MeetupState.Published.CanRaiseEvent(eventType));

        [Theory]
        [InlineData(typeof(Events.MeetupCanceled))]
        [InlineData(typeof(Events.MeetupRsvpOpened))]
        [InlineData(typeof(Events.MeetupMemberWent))]
        public void Given_MeetupState_When_Closed_Then_CanRaiseEvent(Type eventType) =>
            Assert.True(MeetupState.Closed.CanRaiseEvent(eventType));

        [Theory]
        [InlineData(typeof(Events.MeetupRsvpAccepted))]
        [InlineData(typeof(Events.MeetupRsvpDeclined))]
        public void Given_MeetupState_When_Closed_Then_CanNotRaiseEvent(Type eventType) =>
            Assert.False(MeetupState.Closed.CanRaiseEvent(eventType));

        [Theory]
        [InlineData(typeof(Events.MeetupCanceled))]
        [InlineData(typeof(Events.MeetupRsvpClosed))]
        [InlineData(typeof(Events.MeetupRsvpOpened))]
        [InlineData(typeof(Events.MeetupRsvpAccepted))]
        [InlineData(typeof(Events.MeetupRsvpDeclined))]
        [InlineData(typeof(Events.MeetupNumberOfSpotsChanged))]
        public void Given_MeetupState_When_Cancelled_Then_CanNotRaiseEvent(Type eventType) =>
            Assert.False(MeetupState.Cancelled.CanRaiseEvent(eventType));

        [Fact]
        public void Given_Two_MeetupStates_When_Comparing_Then_True()
        {
            var state = MeetupState.Cancelled;

            Assert.True(state == MeetupState.Cancelled);
            Assert.Equal(state, MeetupState.Cancelled);
            Assert.False(state == MeetupState.Closed);
            Assert.NotEqual(state, MeetupState.Closed);
        }
    }
}