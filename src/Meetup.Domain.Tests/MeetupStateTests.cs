using System;
using Meetup.Domain.Events;
using Xunit;

namespace Meetup.Domain.Tests
{
    public class MeetupStateTests 
    {
        [Theory]
        [InlineData(typeof(MeetupCanceledEvent))]
        [InlineData(typeof(MeetupRsvpClosedEvent))]
        [InlineData(typeof(MeetupRsvpAcceptedEvent))]
        [InlineData(typeof(MeetupRsvpDeclinedEvent))]
        public void Given_MeetupState_When_Published_Then_CanRaiseEvent(Type eventType) => 
            Assert.True(MeetupState.Published.CanRaiseEvent(eventType));

        [Theory]
        [InlineData(typeof(MeetupRsvpOpenedEvent))]
        public void Given_MeetupState_When_Published_Then_CanNotRaiseEvent(Type eventType) => 
            Assert.False(MeetupState.Published.CanRaiseEvent(eventType));

        [Theory]
        [InlineData(typeof(MeetupCanceledEvent))]
        [InlineData(typeof(MeetupRsvpOpenedEvent))]
        public void Given_MeetupState_When_Closed_Then_CanRaiseEvent(Type eventType) => 
            Assert.True(MeetupState.Closed.CanRaiseEvent(eventType));

        [Theory]
        [InlineData(typeof(MeetupRsvpAcceptedEvent))]
        [InlineData(typeof(MeetupRsvpDeclinedEvent))]
        public void Given_MeetupState_When_Closed_Then_CanNotRaiseEvent(Type eventType) => 
            Assert.False(MeetupState.Closed.CanRaiseEvent(eventType));

        [Theory]
        [InlineData(typeof(MeetupCanceledEvent))]
        [InlineData(typeof(MeetupRsvpClosedEvent))]
        [InlineData(typeof(MeetupRsvpOpenedEvent))]
        [InlineData(typeof(MeetupRsvpAcceptedEvent))]
        [InlineData(typeof(MeetupRsvpDeclinedEvent))]
        public void Given_MeetupState_When_Cancelled_Then_CanNotRaiseEvent(Type eventType) => 
            Assert.False(MeetupState.Cancelled.CanRaiseEvent(eventType));
    }
}