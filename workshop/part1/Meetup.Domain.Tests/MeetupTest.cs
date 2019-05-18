using System;
using Xunit;

#nullable enable
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
            var numberOfSeats = 50;
            var timeRange = DateTimeRange.From(date: "2019-06-19", time: "19:00", durationInHours: 3);

            var meetup = new Meetup(
                MeetupId.From(id),
                MeetupTitle.From(title));

            Assert.Equal(id, meetup.Id);
            Assert.Equal(title, meetup.Title);

            meetup.UpdateTitle(MeetupTitle.From("CQRS with Postgres"));
            Assert.Equal("CQRS with Postgres", meetup.Title);

            meetup.UpdateLocation(Address.From(address));
            Assert.Equal(address, meetup.Location);

            meetup.UpdateNumberOfSeats(SeatsNumber.From(numberOfSeats));
            Assert.Equal(numberOfSeats, meetup.NumberOfSeats);

            meetup.UpdateTime(timeRange);
            Assert.Equal(timeRange, meetup.TimeRange);
        }
    }
}