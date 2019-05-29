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

        public static void GivenPublishedMeetup<T>(Action<MeetupAggregate> cmd, Action<MeetupAggregate, T> assertEvent) =>
            GivenMeetup(CreateMeetup().Published, cmd, assertEvent);

        public static void GivenPublishedMeetup<T>(Action<MeetupAggregate, T> assertEvent) =>
            GivenMeetup(CreateMeetup().Published, m => { }, assertEvent);

        public static void GivenCreatedMeetup<T>(Action<MeetupAggregate> cmd, Action<MeetupAggregate, T> assertEvent) =>
            GivenMeetup(CreateMeetup, cmd, assertEvent);

        public static void GivenNothing<T>(Func<MeetupAggregate> initState, Action<MeetupAggregate, T> assertEvent) =>
            GivenMeetup(initState, m => { }, assertEvent);

        public static void GivenMeetup<T>(Func<MeetupAggregate> initState, Action<MeetupAggregate> cmd, Action<MeetupAggregate, T> assertEvent)
        {
            var meetup = initState();
            cmd(meetup);

            Assert.IsType<T>(meetup.Events.Last());
            if (meetup.Events.Last() is T ev)
            {
                assertEvent(meetup, ev);
            }
        }

        public static MeetupAggregate CreateMeetup() => new MeetupAggregate(
            MeetupId.From(id),
            MeetupTitle.From(title));

        public static MeetupAggregate AssertProperties(this MeetupAggregate @this)
        {
            Assert.Equal(id, @this.Id);
            Assert.Equal(title, @this.Title);
            Assert.Equal(MeetupState.Created, @this.State);
            return @this;
        }

        public static MeetupAggregate Published(this MeetupAggregate @this) => @this.ExecuteCommand(@this.Publish);

        public static MeetupAggregate Closed(this MeetupAggregate @this) => @this.ExecuteCommand(@this.Close);

        public static MeetupAggregate Canceled(this MeetupAggregate @this) => @this.ExecuteCommand(@this.Cancel);

        private static MeetupAggregate ExecuteCommand(this MeetupAggregate @this, Action command)
        {
            @this.UpdateNumberOfSeats(SeatsNumber.From(numberOfSeats));
            @this.UpdateLocation(Address.From(address));
            @this.UpdateTime(timeRange);
            command();
            return @this;
        }
    }
}