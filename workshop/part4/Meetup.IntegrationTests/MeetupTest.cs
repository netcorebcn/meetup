using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Meetup.IntegrationTests;
using Xunit;
using static Meetup.IntegrationTests.Meetup;
using static Meetup.IntegrationTests.MeetupTestsExtensions;

namespace Meetup.IntegrationTests
{
    public class MeetupTests : IClassFixture<MeetupClientFixture>
    {
        private readonly MeetupClient _client;

        public MeetupTests(MeetupClientFixture fixture) => _client = fixture.MeetupClient;

        [Fact]
        public Task CreateMeetupTest() =>
            _client.TestCase(
                c => c.Create(),
                AssertOk,
                AssertCreated
            );

        [Fact]
        public Task UpdateSeatsTest() =>
            _client.TestCase(
                async c =>
                {
                    await c.Create();
                    return await c.UpdateSeats();
                },
                AssertOk,
                meetup =>
                {
                    AssertCreated(meetup);
                    Assert.Equal(numberOfSeats, meetup.NumberOfSeats);
                }
            );

        [Fact]
        public Task UpdateLocationTest() =>
            _client.TestCase(
                async c =>
                {
                    await c.Create();
                    return await c.UpdateLocation();
                },
                AssertOk,
                meetup =>
                {
                    AssertCreated(meetup);
                    Assert.Equal(location, meetup.Location);
                }
            );

        [Fact]
        public Task Given_Created_Meetup_When_Publish_Then_Error() =>
            _client.TestCase(
                async c =>
                {
                    await c.Create();
                    return await c.Publish();
                },
                AssertError
            );

        [Fact]
        public Task Given_Created_With_All_Properties_Meetup_When_Publish_Then_Published() =>
            _client.TestCase(
                async c =>
                {
                    await c.Create();
                    await c.UpdateLocation();
                    await c.UpdateSeats();
                    await c.UpdateTime();
                    return await c.Publish();
                },
                AssertOk,
                AssertPublished
            );

        [Fact]
        public Task Given_Created_Meetup_When_Cancel_Then_Error() =>
            _client.TestCase(
                async c =>
                {
                    await c.Create();
                    return await c.Cancel();
                },
                AssertError
            );

        [Fact]
        public Task Given_Published_Meetup_When_Cancel_Then_Canceled() =>
            _client.TestCase(
                async c =>
                {
                    await c.Published();
                    return await c.Cancel();
                },
                AssertOk,
                AssertCanceled
            );

        [Fact]
        public Task Given_Created_Meetup_When_Close_Then_Error() =>
            _client.TestCase(
                async c =>
                {
                    await c.Create();
                    return await c.Close();
                },
                AssertError
            );

        [Fact]
        public Task Given_Published_Meetup_When_Close_Then_Closed() =>
            _client.TestCase(
                async c =>
                {
                    await c.Published();
                    return await c.Close();
                },
                AssertOk,
                AssertClosed
            );

        [Fact]
        public async Task Given_Published_Meetup_When_AcceptRSVP_Then_MemberGoing()
        {
            var memberId = Guid.NewGuid();
            var acceptedAt = DateTime.UtcNow;

            await _client.TestCase(
                async c =>
                {
                    await c.Published();
                    return await c.AcceptRSVP(memberId, acceptedAt);
                },
                AssertOk,
                m => Assert.True(m.MembersGoing.ContainsKey(memberId))
            );
        }

        [Fact]
        public async Task Given_Published_Meetup_When_RejectRSVP_Then_MemberNotGoing()
        {
            var memberId = Guid.NewGuid();
            var rejectedAt = DateTime.UtcNow;

            await _client.TestCase(
                async c =>
                {
                    await c.Published();
                    return await c.RejectRSVP(memberId, rejectedAt);
                },
                AssertOk,
                m => Assert.True(m.MembersNotGoing.ContainsKey(memberId))
            );
        }
    }
}