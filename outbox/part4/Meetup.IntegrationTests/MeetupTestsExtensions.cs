using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using static Meetup.IntegrationTests.Meetup;

namespace Meetup.IntegrationTests
{
    public static class MeetupTestsExtensions
    {
        public static Guid id { get; private set; }
        public static readonly string title = "EventSourcing";
        public static readonly int numberOfSeats = 10;
        public static readonly string location = "SanFrancisco, MountainView";
        public static readonly DateTime start = DateTime.ParseExact("2019-08-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);

        public static Task TestCase(
            this MeetupClient @this,
            Func<MeetupClient, Task<HttpResponseMessage>> command,
            Action<HttpResponseMessage> assertResponse = null,
            Action<Meetup> assert = null) =>
            Test(@this, command, assertResponse, assert, @this.Get);

        public static Task TestAttendantsCase(
            this MeetupClient @this,
            Func<MeetupClient, Task<HttpResponseMessage>> command,
            Action<HttpResponseMessage> assertResponse = null,
            Action<Attendants> assert = null) =>
            Test(@this, command, assertResponse, assert, @this.GetAttendants);

        private static async Task Test<TResponse>(
            this MeetupClient @this,
            Func<MeetupClient, Task<HttpResponseMessage>> command,
            Action<HttpResponseMessage> assertResponse,
            Action<TResponse> assert,
            Func<Guid, Task<TResponse>> get)
        {
            assert = assert ?? (_ => { });
            var response = await command(@this);
            assertResponse(response);
            var readModel = await get(id);
            assert(readModel);
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

        public static async Task<HttpResponseMessage> Published(this MeetupClient @this)
        {
            await @this.Create();
            await @this.UpdateLocation();
            await @this.UpdateSeats();
            await @this.UpdateTime();
            return await @this.Publish();
        }

        public static Task<HttpResponseMessage> Publish(this MeetupClient @this) => @this.Publish(id);
        public static Task<HttpResponseMessage> AcceptRSVP(this MeetupClient @this, Guid memberId, DateTime acceptedAt) =>
            @this.AcceptRSVP(id, memberId, acceptedAt);
        public static Task<HttpResponseMessage> RejectRSVP(this MeetupClient @this, Guid memberId, DateTime rejectedAt) =>
            @this.RejectRSVP(id, memberId, rejectedAt);

        public static Task<HttpResponseMessage> Cancel(this MeetupClient @this) => @this.Cancel(id);
        public static Task<HttpResponseMessage> Close(this MeetupClient @this) => @this.Close(id);

        public static void AssertCreated(Meetup meetup)
        {
            AssertBasicProperties(meetup);
            Assert.Equal(MeetupState.Created, meetup.State);
        }

        public static void AssertPublished(Meetup meetup)
        {
            AssertBasicProperties(meetup);
            AssertExtendedProperties(meetup);
            Assert.Equal(MeetupState.Published, meetup.State);
        }

        public static void AssertCanceled(Meetup meetup)
        {
            AssertBasicProperties(meetup);
            AssertExtendedProperties(meetup);
            Assert.Equal(MeetupState.Canceled, meetup.State);
        }

        public static void AssertClosed(Meetup meetup)
        {
            AssertBasicProperties(meetup);
            AssertExtendedProperties(meetup);
            Assert.Equal(MeetupState.Closed, meetup.State);
        }

        private static void AssertExtendedProperties(Meetup meetup)
        {
            Assert.Equal(location, meetup.Location);
            Assert.Equal(numberOfSeats, meetup.NumberOfSeats);
        }

        private static void AssertBasicProperties(Meetup meetup)
        {
            Assert.Equal(id, meetup.Id);
            Assert.Equal(title, meetup.Title);
        }

        public static Task<HttpResponseMessage> UpdateSeats(this MeetupClient @this) =>
            @this.UpdateSeats(id, numberOfSeats);

        public static Task<HttpResponseMessage> UpdateLocation(this MeetupClient @this) =>
            @this.UpdateLocation(id, location);

        public static Task<HttpResponseMessage> UpdateTime(this MeetupClient @this) =>
            @this.UpdateTime(id, start, start.AddHours(2));

        public static async Task<Meetup> Get(this MeetupClient @this) => await @this.Get(id);

        public static void AssertEqual(this List<Guid> list, params Guid[] guids) => Assert.Equal(guids, list);
    }
}