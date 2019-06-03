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

        public static void GivenPublishedMeetupThen<T>(Action<MeetupAggregate> when, Action<T> assert) =>
            GivenMeetupThen<T>(CreateMeetup().Published, when, assert);

        public static void GivenPublishedMeetupThen<T>(Action<MeetupAggregate> when) =>
            GivenMeetupThen<T>(CreateMeetup().Published, when, e => { });

        public static void GivenPublishedMeetupThen<T>(Action<T> assert) =>
            GivenMeetupThen(CreateMeetup().Published, m => { }, assert);

        public static void GivenPublishedMeetupThen<T>() =>
            GivenMeetupThen<T>(CreateMeetup().Published, m => { }, e => { });

        public static void GivenNothingThen<T>(Func<MeetupAggregate> when) =>
            GivenMeetupThen<T>(when, m => { }, e => { });

        public static void GivenMeetupThen<T>(Func<MeetupAggregate> initState, Action<MeetupAggregate> when, Action<T> assert)
        {
            var meetup = initState();
            when(meetup);

            Assert.IsType<T>(meetup.Events.Last());
            if (meetup.Events.Last() is T ev)
            {
                assert(ev);
            }
        }

        public static MeetupAggregate CreateMeetup()
        {
            var meetup = Aggregate<MeetupId>.Build<MeetupAggregate>();
            meetup.Create(MeetupId.From(id), MeetupTitle.From(title));
            return meetup;
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

        public static TEvent AssertEvent<TEvent>(this object @event, Action<TEvent> assert)
        {
            Assert.IsType<TEvent>(@event);
            var ev = (TEvent)@event;
            assert(ev);
            return (TEvent)@event;
        }

        public static void AssertEvent<TEvent>(this object @event) => Assert.IsType<TEvent>(@event);
    }
}