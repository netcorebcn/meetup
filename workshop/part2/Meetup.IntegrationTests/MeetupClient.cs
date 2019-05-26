using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Meetup.IntegrationTests
{
    public class MeetupClient
    {
        private HttpClient _client;

        public MeetupClient(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            client.BaseAddress = new Uri(configuration["meetupUrl"] ?? "http://localhost:5000/api/meetup/");
        }

        public Task Create(Guid id, string title) =>
            _client.PostAsJsonAsync("", new
            {
                Id = id,
                Title = title
            });

        public Task UpdateTitle(Guid id, string title) =>
            _client.PutAsJsonAsync("title", new
            {
                Id = id,
                Title = title
            });

        public Task UpdateSeats(Guid id, int seats) =>
            _client.PutAsJsonAsync("seats", new
            {
                Id = id,
                NumberOfSeats = seats
            });

        public Task UpdateTime(Guid id, DateTime start, DateTime end) =>
            _client.PutAsJsonAsync("time", new
            {
                Id = id,
                Start = start,
                End = end
            });

        public Task UpdateLocation(Guid id, string location) =>
            _client.PutAsJsonAsync("location", new
            {
                Id = id,
                Location = location
            });

        public Task Publish(Guid id) =>
            _client.PutAsJsonAsync("publish", new
            {
                Id = id,
            });

        public Task Cancel(Guid id) =>
            _client.PutAsJsonAsync("cancel", new
            {
                Id = id,
            });

        public Task Close(Guid id) =>
            _client.PutAsJsonAsync("close", new
            {
                Id = id,
            });

        public async Task<Meetup> Get(Guid id)
        {
            var response = await _client.GetAsync($"{id}");
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
            return await response.Content.ReadAsAsync<Meetup>();
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
        public MeetupState State { get; internal set; }

        public enum MeetupState
        {
            Created,
            Published,
            Canceled,
            Closed
        }
    }
}