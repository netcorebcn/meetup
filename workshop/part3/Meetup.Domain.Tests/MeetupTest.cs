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

        [Fact]
        public void Given_Nothing_When_Create_Meetup_Then_Created() =>
            GivenNothing<Events.MeetupCreated>(
                CreateMeetup,
                (m, ev) => Assert.Equal(MeetupState.Created, m.State));

        [Fact]
        public void Given_Created_Meetup_When_Publish_Then_Published() =>
            GivenPublishedMeetup<Events.MeetupPublished>(
                (m, ev) => Assert.Equal(MeetupState.Published, m.State));

        [Fact]
        public void Given_Published_Meetup_When_UpdateSeats_Then_SeatsUpdated() =>
            GivenPublishedMeetup<Events.MeetupNumberOfSeatsUpdated>(
                m => m.UpdateNumberOfSeats(SeatsNumber.From(25)),
                (m, ev) => Assert.Equal(m.NumberOfSeats, ev.NumberOfSeats));

        [Fact]
        public void Given_Valid_Created_Meetup_When_Build_Then_Built()
        {
            var meetup = new Meetup(
                MeetupId.From(id),
                MeetupTitle.From(title),
                Address.None,
                SeatsNumber.None,
                DateTimeRange.None,
                new Dictionary<MemberId, DateTime>(),
                new Dictionary<MemberId, DateTime>(),
                MeetupState.Created);

            Assert.Equal(id, meetup.Id);
            Assert.Equal(title, meetup.Title);
            Assert.Equal(MeetupState.Created, meetup.State);
        }

        [Fact]
        public void Given_Valid_Meetup_When_Build_Then_Built()
        {
            new[] { MeetupState.Created, MeetupState.Published, MeetupState.Canceled, MeetupState.Closed }
            .ToList().ForEach(state =>
            {
                var meetup = new Meetup(
                    MeetupId.From(id),
                    MeetupTitle.From(title),
                    Address.From(address),
                    SeatsNumber.From(numberOfSeats),
                    timeRange,
                    new Dictionary<MemberId, DateTime>(),
                    new Dictionary<MemberId, DateTime>(),
                    state);

                Assert.Equal(id, meetup.Id);
                Assert.Equal(title, meetup.Title);
                Assert.Equal(address, meetup.Location);
                Assert.Equal(numberOfSeats, meetup.NumberOfSeats);
                Assert.Equal(timeRange, meetup.TimeRange);
                Assert.Equal(state, meetup.State);
            });
        }

        [Fact]
        public void Given_Invalid_Meetup_When_Build_Then_Throws()
        {
            new[] { MeetupState.Published, MeetupState.Canceled, MeetupState.Closed }
            .ToList().ForEach(state =>
            {
                Assert.Throws<MeetupDomainException>(() =>
                new Meetup(
                    MeetupId.From(id),
                    MeetupTitle.From(title),
                    Address.None,
                    SeatsNumber.From(numberOfSeats),
                    timeRange,
                    new Dictionary<MemberId, DateTime>(),
                    new Dictionary<MemberId, DateTime>(),
                    state));
            });
        }

        [Fact]
        public void Given_Published_Meetup_When_AcceptRSVP_Then_MemberGoing()
        {
            var memberId = MemberId.From(Guid.NewGuid());
            var acceptedAt = DateTime.UtcNow;

            GivenPublishedMeetup<Events.RSVPAccepted>(
                cmd => cmd.AcceptRSVP(memberId, acceptedAt),
                (m, ev) => Assert.Equal(acceptedAt, m.MembersGoing[memberId]));
        }


        [Fact]
        public void Given_Published_Meetup_When_RejectRSVP_Then_MemberNotGoing()
        {
            var memberId = MemberId.From(Guid.NewGuid());
            var rejectedAt = DateTime.UtcNow;

            GivenPublishedMeetup<Events.RSVPRejected>(
                cmd => cmd.RejectRSVP(memberId, rejectedAt),
                (m, ev) => Assert.Equal(rejectedAt, m.MembersNotGoing[memberId]));
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
    }
}