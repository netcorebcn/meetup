using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xunit;

namespace Meetup.IntegrationTests
{
    public class MeetupClient
    {
        private HttpClient _client;

        public MeetupClient(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            client.BaseAddress = new Uri(configuration["meetupUrl"] ?? "http://localhost:8002/meetup/");
        }

        public Task<HttpResponseMessage> Create(Guid id, string title) =>
            _client.PostAsJsonAsync("", new
            {
                Id = id,
                Title = title
            });

        public Task<HttpResponseMessage> UpdateTitle(Guid id, string title) =>
            _client.PutAsJsonAsync("title", new
            {
                Id = id,
                Title = title
            });

        public Task<HttpResponseMessage> UpdateSeats(Guid id, int seats) =>
            _client.PutAsJsonAsync("seats", new
            {
                Id = id,
                NumberOfSeats = seats
            });

        public Task<HttpResponseMessage> UpdateTime(Guid id, DateTime start, DateTime end) =>
            _client.PutAsJsonAsync("time", new
            {
                Id = id,
                Start = start,
                End = end
            });

        public Task<HttpResponseMessage> UpdateLocation(Guid id, string location) =>
            _client.PutAsJsonAsync("location", new
            {
                Id = id,
                Location = location
            });

        public Task<HttpResponseMessage> Publish(Guid id) =>
            _client.PutAsJsonAsync("publish", new
            {
                Id = id,
            });

        public Task<HttpResponseMessage> AcceptRSVP(Guid id, Guid memberId, DateTime acceptedAt) =>
            _client.PutAsJsonAsync("acceptrsvp", new
            {
                Id = id,
                MemberId = memberId,
                AcceptedAt = acceptedAt
            });

        public Task<HttpResponseMessage> RejectRSVP(Guid id, Guid memberId, DateTime rejectedAt) =>
            _client.PutAsJsonAsync("rejectrsvp", new
            {
                Id = id,
                MemberId = memberId,
                RejectedAt = rejectedAt
            });

        public Task<HttpResponseMessage> Cancel(Guid id) =>
            _client.PutAsJsonAsync("cancel", new
            {
                Id = id,
            });

        public Task<HttpResponseMessage> Close(Guid id) =>
            _client.PutAsJsonAsync("close", new
            {
                Id = id,
            });

        public async Task<Meetup> Get(Guid id) => await Get<Meetup>(id);

        public async Task<Attendants> GetAttendants(Guid id) => await Get<Attendants>(id, "attendants/");

        public async Task<TResponse> Get<TResponse>(Guid id, string uri = "")
        {
            var response = await _client.GetAsync($"{uri}{id}");
            var log = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Get RESPONSE: {log}");
            var content = await response.Content.ReadAsStringAsync();
            return await response.Content.ReadAsAsync<TResponse>();
        }
    }

    public class Meetup
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public int NumberOfSeats { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public MeetupState State { get; set; }
        public Dictionary<Guid, DateTime> MembersGoing { get; set; }
        public Dictionary<Guid, DateTime> MembersNotGoing { get; set; }

        public enum MeetupState
        {
            Created,
            Published,
            Canceled,
            Closed
        }
    }

    public class Attendants
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<Guid> Going { get; set; } = new List<Guid>();
        public List<Guid> NotGoing { get; set; } = new List<Guid>();
        public List<Guid> Waiting { get; set; } = new List<Guid>();
        public int NumberOfSeats { get; set; }
    }
}