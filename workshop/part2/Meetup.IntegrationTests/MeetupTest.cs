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
        public Task Given_Invalid_Meetup_When_Publish_Then_Error() =>
            _client.TestCase(
                async c =>
                {
                    await c.Create();
                    return await c.Publish();
                },
                AssertError
            );

        [Fact]
        public Task Given_Invalid_Meetup_When_Publish_Then_Published() =>
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
    }

    public static class MeetupTestsExtensions
    {
        public static Guid id { get; private set; }
        public static readonly string title = "EventSourcing";
        public static readonly int numberOfSeats = 10;
        public static readonly string location = "SanFrancisco, MountainView";
        public static readonly DateTime start = DateTime.ParseExact("2019-08-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);

        public static async Task TestCase(
            this MeetupClient @this,
            Func<MeetupClient, Task<HttpResponseMessage>> command,
            Action<HttpResponseMessage> assertResponse = null,
            Action<Meetup> assert = null)
        {
            assert = assert ?? (_ => { });

            var response = await command(@this);
            assertResponse(response);

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
            await @this.Get(id);
        }

        public static void AssertOk(HttpResponseMessage response) =>
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        public static void AssertError(HttpResponseMessage response) =>
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        public static Task<HttpResponseMessage> Create(this MeetupClient @this)
        {
            id = Guid.NewGuid();
            return @this.Create(id, title);
        }

        public static Task<HttpResponseMessage> Publish(this MeetupClient @this) => @this.Publish(id);

        public static void AssertCreated(Meetup meetup)
        {
            Assert.Equal(id, meetup.Id);
            Assert.Equal(title, meetup.Title);
            Assert.Equal(MeetupState.Created, meetup.State);
        }

        public static void AssertPublished(Meetup meetup)
        {
            Assert.Equal(id, meetup.Id);
            Assert.Equal(title, meetup.Title);
            Assert.Equal(location, meetup.Location);
            Assert.Equal(numberOfSeats, meetup.NumberOfSeats);
            Assert.Equal(MeetupState.Published, meetup.State);
        }

        public static Task<HttpResponseMessage> UpdateSeats(this MeetupClient @this) =>
            @this.UpdateSeats(id, numberOfSeats);

        public static Task<HttpResponseMessage> UpdateLocation(this MeetupClient @this) =>
            @this.UpdateLocation(id, location);

        public static Task<HttpResponseMessage> UpdateTime(this MeetupClient @this) =>
            @this.UpdateTime(id, start, start.AddHours(2));

        public static async Task<Meetup> Get(this MeetupClient @this) => await @this.Get(id);

    }
}