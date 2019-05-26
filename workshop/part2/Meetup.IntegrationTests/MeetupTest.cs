using System;
using System.Globalization;
using System.Threading.Tasks;
using Meetup.IntegrationTests;
using Xunit;
using static Meetup.IntegrationTests.Meetup;

namespace Meetup.IntegrationTests
{
    public class MeetupTests : IClassFixture<MeetupClientFixture>
    {
        private readonly MeetupClient _client;

        public MeetupTests(MeetupClientFixture fixture) => _client = fixture.MeetupClient;

        [Fact]
        public async Task CreateMeetupTest()
        {
            var id = Guid.NewGuid();
            const string title = "EventSourcing";
            const int numberOfSeats = 10;
            const string location = "SanFrancisco, MountainView";
            var start = DateTime.ParseExact("2019-08-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);

            await _client.Create(id, title);
            await _client.UpdateSeats(id, numberOfSeats);
            await _client.UpdateLocation(id, location);
            await _client.UpdateTime(id, start, start.AddHours(2));
            await _client.Publish(id);

            var meetup = await _client.Get(id);
            Assert.Equal(id, meetup.Id);
            Assert.Equal(title, meetup.Title);
            Assert.Equal(numberOfSeats, meetup.NumberOfSeats);
            Assert.Equal(location, meetup.Location);
            // Assert.Equal(start, meetup.Start);
            Assert.Equal(MeetupState.Published, meetup.State);
        }
    }
}
