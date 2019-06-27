using System;
using Xunit;

#nullable enable
namespace Meetup.Domain.Tests
{
    public class MeetupStateTest
    {
        [Theory]
        [InlineData(typeof(Events.MeetupCreated))]
        [InlineData(typeof(Events.MeetupPublished))]
        [InlineData(typeof(Events.MeetupTitleUpdated))]
        [InlineData(typeof(Events.MeetupNumberOfSeatsUpdated))]
        [InlineData(typeof(Events.MeetupLocationUpdated))]
        [InlineData(typeof(Events.MeetupTimeUpdated))]
        public void Given_Created_Meetup_When_CheckEventType_Then_CanRaiseEvent(Type eventType)
        {
            MeetupState.Created.EnsureCanRaiseEvent(eventType);
            Assert.True(MeetupState.Created.CanRaiseEvent(eventType));
        }

        [Theory]
        [InlineData(typeof(Events.MeetupCanceled))]
        [InlineData(typeof(Events.MeetupClosed))]
        public void Given_Created_Meetup_When_CheckEventType_Then_CanNotRaiseEvent(Type eventType)
        {
            Assert.Throws<MeetupDomainException>(() => MeetupState.Created.EnsureCanRaiseEvent(eventType));
            Assert.False(MeetupState.Created.CanRaiseEvent(eventType));
        }

        [Theory]
        [InlineData(typeof(Events.MeetupPublished))]
        [InlineData(typeof(Events.MeetupTitleUpdated))]
        [InlineData(typeof(Events.MeetupNumberOfSeatsUpdated))]
        [InlineData(typeof(Events.MeetupLocationUpdated))]
        [InlineData(typeof(Events.MeetupTimeUpdated))]
        [InlineData(typeof(Events.MeetupClosed))]
        [InlineData(typeof(Events.MeetupCanceled))]
        [InlineData(typeof(Events.RSVPAccepted))]
        [InlineData(typeof(Events.RSVPRejected))]
        public void Given_Published_Meetup_When_CheckEventType_Then_CanRaiseEvent(Type eventType)
        {
            MeetupState.Published.EnsureCanRaiseEvent(eventType);
            Assert.True(MeetupState.Published.CanRaiseEvent(eventType));
        }

        [Theory]
        [InlineData(typeof(Events.MeetupCreated))]
        public void Given_Published_Meetup_When_CheckEventType_Then_CanNotRaiseEvent(Type eventType)
        {
            Assert.Throws<MeetupDomainException>(() => MeetupState.Published.EnsureCanRaiseEvent(eventType));
            Assert.False(MeetupState.Published.CanRaiseEvent(eventType));
        }

        [Theory]
        [InlineData(typeof(Events.MeetupClosed))]
        public void Given_Closed_Meetup_When_CheckEventType_Then_CanRaiseEvent(Type eventType)
        {
            MeetupState.Closed.EnsureCanRaiseEvent(eventType);
            Assert.True(MeetupState.Closed.CanRaiseEvent(eventType));
        }

        [Theory]
        [InlineData(typeof(Events.MeetupCreated))]
        [InlineData(typeof(Events.MeetupPublished))]
        [InlineData(typeof(Events.MeetupNumberOfSeatsUpdated))]
        [InlineData(typeof(Events.MeetupLocationUpdated))]
        [InlineData(typeof(Events.MeetupTitleUpdated))]
        [InlineData(typeof(Events.RSVPAccepted))]
        [InlineData(typeof(Events.RSVPRejected))]
        public void Given_Closed_Meetup_When_CheckEventType_Then_CanNotRaiseEvent(Type eventType)
        {
            Assert.Throws<MeetupDomainException>(() => MeetupState.Closed.EnsureCanRaiseEvent(eventType));
            Assert.False(MeetupState.Closed.CanRaiseEvent(eventType));
        }

        [Theory]
        [InlineData(typeof(Events.MeetupCanceled))]
        public void Given_Canceled_Meetup_When_CheckEventType_Then_CanRaiseEvent(Type eventType)
        {
            MeetupState.Canceled.EnsureCanRaiseEvent(eventType);
            Assert.True(MeetupState.Canceled.CanRaiseEvent(eventType));
        }

        [Theory]
        [InlineData(typeof(Events.MeetupCreated))]
        [InlineData(typeof(Events.MeetupClosed))]
        [InlineData(typeof(Events.MeetupPublished))]
        [InlineData(typeof(Events.MeetupNumberOfSeatsUpdated))]
        [InlineData(typeof(Events.MeetupLocationUpdated))]
        [InlineData(typeof(Events.MeetupTitleUpdated))]
        [InlineData(typeof(Events.RSVPAccepted))]
        [InlineData(typeof(Events.RSVPRejected))]
        public void Given_Canceled_Meetup_When_CheckEventType_Then_CanNotRaiseEvent(Type eventType)
        {
            Assert.Throws<MeetupDomainException>(() => MeetupState.Canceled.EnsureCanRaiseEvent(eventType));
            Assert.False(MeetupState.Canceled.CanRaiseEvent(eventType));
        }
    }
}