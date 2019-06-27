using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Meetup.Domain.Tests.MeetupProjectionTestExtensions;

#nullable enable
namespace Meetup.Domain.Tests
{
    public class AttendantsProjectionTest
    {
        [Fact]
        public void Given_ThreeGoing_When_Only_TwoSeats_Then_OneWaiting() =>
            TestCase(
                project: projection => projection
                    .WhenPublishedMeetup()
                    .WhenMemberAccepted(bob)
                    .WhenMemberAccepted(jon)
                    .WhenMemberAccepted(carla)
                    .WhenMemberRejected(sara),
                assert: readModel =>
                {
                    readModel.Going.AssertEqual(bob, jon);
                    readModel.Waiting.AssertEqual(carla);
                    readModel.NotGoing.AssertEqual(sara);
                }
            );

        [Fact]
        public void Given_ThreeGoing_With_TwoSeats_When_SomeoneRejects_Then_EmptyWaiting() =>
            TestCase(
                project: projection => projection
                    .WhenPublishedMeetup()
                    .WhenMemberAccepted(bob)
                    .WhenMemberAccepted(jon)
                    .WhenMemberAccepted(carla)
                    .WhenMemberRejected(bob),
                assert: readModel =>
                {
                    readModel.Going.AssertEqual(jon, carla);
                    readModel.NotGoing.AssertEqual(bob);
                    Assert.Empty(readModel.Waiting);
                }
            );


        [Fact]
        public void Given_Rejected_Rsvp_When_Accept_Then_Empty_NotGoing() =>
            TestCase(
                project: projection => projection
                    .WhenPublishedMeetup()
                    .WhenMemberRejected(bob)
                    .WhenMemberAccepted(bob),
                assert: readModel =>
                {
                    readModel.Going.AssertEqual(bob);
                    Assert.Empty(readModel.Waiting);
                    Assert.Empty(readModel.NotGoing);
                }
            );

        [Fact]
        public void Given_Accepted_Rsvp_When_Reject_Then_Empty_Going() =>
            TestCase(
                project: projection => projection
                    .WhenPublishedMeetup()
                    .WhenMemberAccepted(bob)
                    .WhenMemberRejected(bob),
                assert: readModel =>
                {
                    readModel.NotGoing.AssertEqual(bob);
                    Assert.Empty(readModel.Going);
                    Assert.Empty(readModel.Waiting);
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
        private static AttendantsReadModel readModel;

        public readonly static (Guid memberId, DateTime rsvpAt)
            bob = (Guid.NewGuid(), DateTime.UtcNow),
            jon = (Guid.NewGuid(), DateTime.UtcNow.AddSeconds(1)),
            sara = (Guid.NewGuid(), DateTime.UtcNow.AddSeconds(1)),
            carla = (Guid.NewGuid(), DateTime.UtcNow.AddSeconds(1));

        public static void TestCase(Action<AttendantsProjection> project, Action<AttendantsReadModel> assert)
        {
            var projection = new AttendantsProjection();
            readModel = new AttendantsReadModel();
            project(projection);
            assert(readModel);

            Assert.Equal(meetupId, readModel.Id);
            Assert.Equal(title, readModel.Title);
        }

        public static AttendantsProjection WhenPublishedMeetup(this AttendantsProjection projection, params object[] events)
        {
            projection.Project(
                readModel,
                new Events.MeetupCreated(meetupId, title),
                new Events.MeetupNumberOfSeatsUpdated(meetupId, numberOfSeats),
                new Events.MeetupLocationUpdated(meetupId, location),
                new Events.MeetupTimeUpdated(meetupId, start, end),
                new Events.MeetupPublished(meetupId)
            );
            return projection;
        }

        public static AttendantsProjection WhenNumberOfSeatsUpdated(this AttendantsProjection projection, int seats)
        {
            projection.Project(readModel, new Events.MeetupNumberOfSeatsUpdated(meetupId, seats));
            return projection;
        }

        public static AttendantsProjection WhenMemberAccepted(this AttendantsProjection projection, (Guid memberId, DateTime rsvpAt) member)
        {
            projection.Project(readModel, new Events.RSVPAccepted(meetupId, member.memberId, member.rsvpAt));
            return projection;
        }

        public static AttendantsProjection WhenMemberRejected(this AttendantsProjection projection, (Guid memberId, DateTime rsvpAt) member)
        {
            projection.Project(readModel, new Events.RSVPRejected(meetupId, member.memberId, member.rsvpAt));
            return projection;
        }

        public static void AssertEqual(this List<Guid> list, params (Guid memberId, DateTime at)[] guids) => Assert.Equal(guids.Select(x => x.memberId), list);
    }
}