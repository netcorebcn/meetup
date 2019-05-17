using System;
using Xunit;

namespace Meetup.Domain.Tests
{
    public class MeetupTest
    {
        [Fact]
        public void Given_NewMeetup_When_Create_Then_Created()
        {
            var id = Guid.NewGuid();
            var title = "EventSourcing with Postgres";
            var address = "Skills Matters, London";

            var meetup = new Meetup(
                MeetupId.From(id),
                MeetupTitle.From(title),
                Address.From(address),
                SeatsNumber.From(50),
                DateTimeRange.From(new DateTime(2019, 6, 19, 19, 00, 00), durationInHours: 3));

            Assert.Equal(id, meetup.Id);
            Assert.Equal(title, meetup.Title);
            Assert.Equal(address, meetup.Location);
        }
    }
}