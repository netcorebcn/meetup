using System;
using System.Linq;
using Xunit;
using static Meetup.Domain.Tests.MeetupProjectionTestExtensions;

#nullable enable
namespace Meetup.Domain.Tests
{
    public class MeetupProjectionTest
    {
        [Fact]
        public void Given_ThreeGoing_With_TwoSeats_When_Project_Then_OneWaiting() =>
            TestCase(
                project: projection => projection
                    .WhenPublishedMeetup()
                    .WhenMemberAccepted(bob)
                    .WhenMemberAccepted(jon)
                    .WhenMemberAccepted(carla)
                    .WhenMemberRejected(sara),
                assert: readModel =>
                {
                    Assert.Equal(jon.memberId, readModel.Going.Last());
                    Assert.Equal(carla.memberId, readModel.Waiting.Last());
                    Assert.Equal(sara.memberId, readModel.NotGoing.Last());
                }
            );
    }

    public static class MeetupProjectionTestExtensions
    {
        public readonly static Guid meetupId = Guid.NewGuid();
        public readonly static string title = "EventSourcing";
        public readonly static string location = "SanFrancisco,California";
        public readonly static DateTime start = DateTime.UtcNow;
        public readonly static DateTime end = DateTime.UtcNow.AddHours(3);
        public readonly static int numberOfSeats = 2;

        public readonly static (Guid memberId, DateTime rsvpAt)
            bob = (Guid.NewGuid(), DateTime.UtcNow),
            jon = (Guid.NewGuid(), DateTime.UtcNow.AddSeconds(1)),
            sara = (Guid.NewGuid(), DateTime.UtcNow.AddSeconds(1)),
            carla = (Guid.NewGuid(), DateTime.UtcNow.AddSeconds(1));

        public static void TestCase(Action<MeetupProjection> project, Action<MeetupReadModel> assert)
        {
            var projection = new MeetupProjection();
            project(projection);
            var readModel = projection.ReadModel();
            assert(readModel);

            Assert.Equal(meetupId, readModel.Id);
            Assert.Equal(title, readModel.Title);
        }

        public static MeetupProjection WhenPublishedMeetup(this MeetupProjection projection, params object[] events)
        {
            projection.Project(
                new Events.MeetupCreated(meetupId, title),
                new Events.MeetupNumberOfSeatsUpdated(meetupId, numberOfSeats),
                new Events.MeetupLocationUpdated(meetupId, location),
                new Events.MeetupTimeUpdated(meetupId, start, end),
                new Events.MeetupPublished(meetupId)
            );
            return projection;
        }

        public static MeetupProjection WhenMemberAccepted(this MeetupProjection projection, (Guid memberId, DateTime rsvpAt) member)
        {
            projection.Project(new Events.RSVPAccepted(meetupId, member.memberId, member.rsvpAt));
            return projection;
        }

        public static MeetupProjection WhenMemberRejected(this MeetupProjection projection, (Guid memberId, DateTime rsvpAt) member)
        {
            projection.Project(new Events.RSVPRejected(meetupId, member.memberId, member.rsvpAt));
            return projection;
        }

        public static MeetupReadModel ReadModel(this MeetupProjection projection) => projection.Project();
    }
}