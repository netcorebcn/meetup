using System;
using System.Linq;
using Xunit;

namespace Meetup.Domain.Tests
{
    public static class MeetupTestExtensions
    {
        public readonly static Guid id = Guid.NewGuid();
        public readonly static string title = "EventSourcing with Postgres";
        public readonly static string address = "Skills Matters, London";
        public readonly static int numberOfSeats = 50;
        public readonly static DateTimeRange timeRange = DateTimeRange.From(date: "2019-06-19", time: "19:00", durationInHours: 3);

        public static void GivenPublishedMeetup<T>(Action<Meetup> cmd, Action<Meetup, T> assertEvent) =>
            GivenMeetup(CreateMeetup().Published, cmd, assertEvent);
        public static void GivenPublishedMeetup<T>(Action<Meetup, T> assertEvent) =>
            GivenMeetup(CreateMeetup().Published, m => { }, assertEvent);

        public static void GivenCreatedMeetup<T>(Action<Meetup> cmd, Action<Meetup, T> assertEvent) =>
            GivenMeetup(CreateMeetup, cmd, assertEvent);

        public static void GivenNothing<T>(Func<Meetup> initState, Action<Meetup, T> assertEvent) =>
            GivenMeetup(initState, m => { }, assertEvent);

        public static void GivenMeetup<T>(Func<Meetup> initState, Action<Meetup> cmd, Action<Meetup, T> assertEvent)
        {
            var meetup = initState();
            cmd(meetup);

            Assert.IsType<T>(meetup.Events.Last());
            if (meetup.Events.Last() is T ev)
            {
                assertEvent(meetup, ev);
            }
        }

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