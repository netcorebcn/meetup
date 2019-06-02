using System;
using Xunit;
using static Meetup.Domain.Tests.MeetupTestExtensions;
using Meetup.Domain;
using System.Linq;
using System.Collections.Generic;

#nullable enable
namespace Meetup.Domain.Tests
{
    public class MeetupTest
    {
        [Fact]
        public void Given_Created_Meetup_When_Create_Then_Created()
        {
            var meetup = CreateMeetup();
            meetup.UpdateTitle(MeetupTitle.From("CQRS with Postgres"));
            meetup.UpdateLocation(Address.From(address));
            meetup.UpdateNumberOfSeats(SeatsNumber.From(numberOfSeats));
            meetup.UpdateTime(timeRange);

            var events = meetup.Events.ToArray();
            events[0].AssertEvent<Events.MeetupCreated>(
                ev => Assert.Equal(id, ev.Id));
            events[1].AssertEvent<Events.MeetupTitleUpdated>(
                ev => Assert.Equal("CQRS with Postgres", ev.Title));
            events[2].AssertEvent<Events.MeetupLocationUpdated>(
                ev => Assert.Equal(address, ev.Location));
            events[3].AssertEvent<Events.MeetupNumberOfSeatsUpdated>(
                ev => Assert.Equal(numberOfSeats, ev.NumberOfSeats));
            events[4].AssertEvent<Events.MeetupTimeUpdated>(
                ev => Assert.Equal(timeRange, DateTimeRange.From(ev.Start, ev.End)));
        }

        [Fact]
        public void Given_Valid_Meetup_When_Publish_Then_Published()
        {
            var meetup = CreateMeetup();

            meetup.UpdateNumberOfSeats(SeatsNumber.From(numberOfSeats));
            meetup.UpdateLocation(Address.From(address));
            meetup.UpdateTime(timeRange);
            meetup.Publish();

            meetup.Events.Last().AssertEvent<Events.MeetupPublished>();
        }

        [Fact]
        public void Given_Invalid_Meetup_When_Publish_Then_Throws()
        {
            var meetup = CreateMeetup();
            Assert.Throws<MeetupDomainException>(() => meetup.Publish());
        }

        [Fact]
        public void Given_Published_Meetup_When_Cancel_Then_Canceled()
        {
            var meetup = CreateMeetup().Published();
            meetup.Cancel();

            meetup.Events.Last().AssertEvent<Events.MeetupCanceled>();
        }

        [Fact]
        public void Given_Created_Meetup_When_Cancel_Then_Throws() =>
            Assert.Throws<MeetupDomainException>(() => CreateMeetup().Canceled());

        [Fact]
        public void Given_Published_Meetup_When_Close_Then_Closed()
        {
            var meetup = CreateMeetup().Published().Closed();
            meetup.Events.Last().AssertEvent<Events.MeetupClosed>();
        }

        [Fact]
        public void Given_Published_Meetup_When_Publish_Then_Nothing()
        {
            var meetup = CreateMeetup().Published().Published();
            meetup.Events.Last().AssertEvent<Events.MeetupPublished>();
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

        [Fact]
        public void Given_Nothing_When_Create_Meetup_Then_Created() =>
            GivenNothingThen<Events.MeetupCreated>(when: CreateMeetup);

        [Fact]
        public void Given_Created_Meetup_When_Publish_Then_Published() =>
            GivenPublishedMeetupThen<Events.MeetupPublished>();

        [Fact]
        public void Given_Published_Meetup_When_UpdateSeats_Then_SeatsUpdated() =>
            GivenPublishedMeetupThen<Events.MeetupNumberOfSeatsUpdated>(
                when: m => m.UpdateNumberOfSeats(SeatsNumber.From(25)),
                assert: ev => Assert.Equal(25, ev.NumberOfSeats));

        [Fact]
        public void Given_Published_Meetup_When_AcceptRSVP_Then_MemberGoing()
        {
            var memberId = MemberId.From(Guid.NewGuid());
            var acceptedAt = DateTime.UtcNow;
            GivenPublishedMeetupThen<Events.RSVPAccepted>(when: m => m.AcceptRSVP(memberId, acceptedAt));
        }

        [Fact]
        public void Given_Published_Meetup_When_RejectRSVP_Then_MemberNotGoing()
        {
            var memberId = MemberId.From(Guid.NewGuid());
            var rejectedAt = DateTime.UtcNow;
            GivenPublishedMeetupThen<Events.RSVPRejected>(when: m => m.RejectRSVP(memberId, rejectedAt));
        }

        [Fact]
        public void Given_NotPublished_Meetup_When_AcceptOrRejectRSVP_Then_Throws()
        {
            var memberId = MemberId.From(Guid.NewGuid());
            var time = DateTime.UtcNow;
            var meetup = CreateMeetup();
            Assert.Throws<MeetupDomainException>(() => meetup.AcceptRSVP(memberId, time));
            Assert.Throws<MeetupDomainException>(() => meetup.RejectRSVP(memberId, time));
        }

        [Fact]
        public void Given_Closed_Meetup_When_AcceptOrRejectRSVP_Then_Throws()
        {
            var memberId = MemberId.From(Guid.NewGuid());
            var time = DateTime.UtcNow;
            var meetup = CreateMeetup().Published().Closed();

            Assert.Throws<MeetupDomainException>(() => meetup.AcceptRSVP(memberId, time));
            Assert.Throws<MeetupDomainException>(() => meetup.RejectRSVP(memberId, time));
        }

        [Fact]
        public void Given_Canceled_Meetup_When_AcceptOrRejectRSVP_Then_Throws()
        {
            var memberId = MemberId.From(Guid.NewGuid());
            var time = DateTime.UtcNow;
            var meetup = CreateMeetup().Published().Canceled();

            Assert.Throws<MeetupDomainException>(() => meetup.AcceptRSVP(memberId, time));
            Assert.Throws<MeetupDomainException>(() => meetup.RejectRSVP(memberId, time));
        }

        [Fact]
        public void Given_ValidEvents_When_Build_Then_Built()
        {
            var id = MeetupId.From(Guid.NewGuid());
            var meetup = MeetupAggregate.Build(
                id,
                new Events.MeetupCreated(id, "EventSourcing with Marten"),
                new Events.MeetupNumberOfSeatsUpdated(id, 10),
                new Events.MeetupLocationUpdated(id, "Barcelona"),
                new Events.MeetupTimeUpdated(id, DateTime.UtcNow, DateTime.UtcNow.AddHours(2)),
                new Events.MeetupPublished(id)
            );

            Assert.Equal(id, meetup.Id);
        }

        [Fact]
        public void Given_Invalid_EvenStream_When_Build_Then_Throws()
        {
            var id = MeetupId.From(Guid.NewGuid());
            Assert.Throws<MeetupDomainException>(() =>
                MeetupAggregate.Build(
                    id,
                    new Events.MeetupCreated(id, "EventSourcing with Marten"),
                    new Events.MeetupNumberOfSeatsUpdated(id, 10),
                    new Events.MeetupLocationUpdated(id, "Barcelona"),
                    new Events.MeetupPublished(id),
                    new Events.MeetupTimeUpdated(id, DateTime.UtcNow, DateTime.UtcNow.AddHours(2))
            ));
        }
    }
}