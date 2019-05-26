using System;
using System.Threading.Tasks;
using Meetup.IntegrationTests;
using Xunit;

namespace Meetup.Domain.IntegrationTests
{
    public class MeetupTests : IClassFixture<MeetupClientFixture>
    {
        private readonly MeetupClient _client;

        public MeetupTests(MeetupClientFixture fixture) => _client = fixture.MeetupClient;

        [Fact]
        public async Task CreateMeetupTest()
        {
            var id = Guid.NewGuid();
            await _client.Create(id, "EventSourcing");
            var meetup = await _client.Get(id);
            Assert.Equal(id, meetup.Id);

        }
    }
}
