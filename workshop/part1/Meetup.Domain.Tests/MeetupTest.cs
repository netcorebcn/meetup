using System;
using Xunit;
using static Meetup.Domain.Tests.MeetupTestExtensions;
using Meetup.Domain;

#nullable enable
namespace Meetup.Domain.Tests
{
    public class MeetupTest
    {
        [Fact]
        public void Given_Created_Meetup_When_Create_Then_Created()
        {
            var meetup = CreateMeetup().AssertProperties();

            meetup.UpdateTitle(MeetupTitle.From("CQRS with Postgres"));
            Assert.Equal("CQRS with Postgres", meetup.Title);

            meetup.UpdateLocation(Address.From(address));
            Assert.Equal(address, meetup.Location);

            meetup.UpdateNumberOfSeats(SeatsNumber.From(numberOfSeats));
            Assert.Equal(numberOfSeats, meetup.NumberOfSeats);

            meetup.UpdateTime(timeRange);
            Assert.Equal(timeRange, meetup.TimeRange);
        }

        [Fact]
        public void Given_Valid_Meetup_When_Publish_Then_Published()
        {
            var meetup = CreateMeetup().AssertProperties();

            meetup.UpdateNumberOfSeats(SeatsNumber.From(numberOfSeats));
            meetup.UpdateLocation(Address.From(address));
            meetup.UpdateTime(timeRange);

            meetup.Publish();
            Assert.Equal(MeetupState.Published, meetup.State);
        }

        [Fact]
        public void Given_Invalid_Meetup_When_Publish_Then_Throws()
        {
            var meetup = CreateMeetup().AssertProperties();
            Assert.Throws<MeetupDomainException>(() => meetup.Publish());
        }

        [Fact]
        public void Given_Published_Meetup_When_Cancel_Then_Canceled()
        {
            var meetup = CreateMeetup().Published();
            meetup.Cancel();
            Assert.Equal(MeetupState.Canceled, meetup.State);
        }

        [Fact]
        public void Given_Created_Meetup_When_Cancel_Then_Throws() =>
            Assert.Throws<MeetupDomainException>(() => CreateMeetup().Canceled());

        [Fact]
        public void Given_Published_Meetup_When_Close_Then_Closed()
        {
            var meetup = CreateMeetup().Published().Closed();
            Assert.Equal(MeetupState.Closed, meetup.State);
        }

        [Fact]
        public void Given_Published_Meetup_When_Publish_Then_Nothing()
        {
            var meetup = CreateMeetup().Published().Published();
            Assert.Equal(MeetupState.Published, meetup.State);
        }

        [Fact]
        public void Given_Created_Meetup_When_Close_Then_Throws() =>
            Assert.Throws<MeetupDomainException>(() => CreateMeetup().Closed());

        [Fact]
        public void Given_Closed_Meetup_When_Canceled_Then_Throws() =>
            Assert.Throws<MeetupDomainException>(() => CreateMeetup()
                .Published()
                .Closed()
                .Canceled());

        [Fact]
        public void Given_Canceled_Meetup_When_Close_Then_Throws() =>
            Assert.Throws<MeetupDomainException>(() => CreateMeetup()
                .Published()
                .Canceled()
                .Closed());
    }

    public static class MeetupTestExtensions
    {
        public readonly static Guid id = Guid.NewGuid();
        public readonly static string title = "EventSourcing with Postgres";
        public readonly static string address = "Skills Matters, London";
        public readonly static int numberOfSeats = 50;
        public readonly static DateTimeRange timeRange = DateTimeRange.From(date: "2019-06-19", time: "19:00", durationInHours: 3);

        public static Meetup CreateMeetup() => new Meetup(
            MeetupId.From(id),
            MeetupTitle.From(title));
        public static Meetup AssertProperties(this Meetup @this)
        {
            Assert.Equal(id, @this.Id);
            Assert.Equal(title, @this.Title);
            Assert.Equal(MeetupState.Created, @this.State);
            return @this;
        }

        public static Meetup Published(this Meetup @this) => @this.ExecuteCommand(@this.Publish);

        public static Meetup Closed(this Meetup @this) => @this.ExecuteCommand(@this.Close);

        public static Meetup Canceled(this Meetup @this) => @this.ExecuteCommand(@this.Cancel);

        private static Meetup ExecuteCommand(this Meetup @this, Action command)
        {
            @this.UpdateNumberOfSeats(SeatsNumber.From(numberOfSeats));
            @this.UpdateLocation(Address.From(address));
            @this.UpdateTime(timeRange);
            command();
            return @this;
        }
    }
}