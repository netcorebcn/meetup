using System;
using Xunit;

#nullable enable
namespace Meetup.Domain.Tests
{
    public class MeetupStateTest
    {
        [Fact]
        public void Given_Created_Meetup_When_Create_Then_Created()
        {
            var state = MeetupState.Created;

            Assert.True(MeetupState.Created.CanRaiseEvent<Events.MeetupTitleUpdated>());
            Assert.False(MeetupState.Created.CanRaiseEvent<Events.RSVPAccepted>());
        }
    }
}