using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meetup.Domain;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Linq;

namespace Meetup.Api
{
    public class MeetupDbRepository : IMeetupRepository
    {
        private readonly IMongoCollection<MeetupDocument> _meetups;

        public MeetupDbRepository(IConfiguration configuration)
        {
            var connectionString = configuration["mongodb"] ?? throw new ArgumentNullException("Missing mongodb configuration connection string");

            var client = new MongoClient(connectionString);
            var db = client.GetDatabase("meetup");
            _meetups = db.GetCollection<MeetupDocument>("meetupevents");
        }

        public async Task<Domain.Meetup> Get(Guid id)
        {
            var doc = await _meetups
            .Find<MeetupDocument>(meetup => meetup.Id == id)
            .FirstOrDefaultAsync();

            return doc != null
            ? new Domain.Meetup(
                    MeetupId.From(id),
                    MeetupTitle.From(doc.Title),
                    Address.From(doc.Location),
                    SeatsNumber.From(doc.NumberOfSeats),
                    DateTimeRange.From(doc.Start, doc.End),
                    doc.MembersGoing.ToDictionary(k => MemberId.From(Guid.Parse(k.Key)), v => v.Value),
                    doc.MembersNotGoing.ToDictionary(k => MemberId.From(Guid.Parse(k.Key)), v => v.Value),
                    doc.State.ToLower() switch
                    {
                        "created" => MeetupState.Created,
                        "published" => MeetupState.Published,
                        "closed" => MeetupState.Closed,
                        "canceled" => MeetupState.Canceled,
                        _ => MeetupState.Created,
                    })
            : null;
        }

        public async Task Save(Domain.Meetup entity) =>
            await _meetups.ReplaceOneAsync(meetup => meetup.Id == entity.Id, new MeetupDocument
            {
                Id = entity.Id,
                Title = entity.Title,
                Location = entity.Location,
                NumberOfSeats = entity.NumberOfSeats,
                Start = entity.TimeRange.Start,
                End = entity.TimeRange.End,
                State = entity.State.ToString(),
                MembersGoing = entity.MembersGoing.ToDictionary(x => x.Key.Value.ToString(), y => y.Value),
                MembersNotGoing = entity.MembersNotGoing.ToDictionary(x => x.Key.Value.ToString(), y => y.Value),
            }, new UpdateOptions
            {
                IsUpsert = true
            });
    }

    public class MeetupDocument
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("Location")]
        public string Location { get; set; }

        [BsonElement("NumberOfSeats")]
        public int NumberOfSeats { get; set; }

        [BsonElement("Start")]
        public DateTime Start { get; set; }

        [BsonElement("End")]
        public DateTime End { get; set; }

        [BsonElement("State")]
        public string State { get; set; }

        [BsonElement("MembersGoing")]
        public Dictionary<string, DateTime> MembersGoing { get; set; }

        [BsonElement("MembersNotGoing")]
        public Dictionary<string, DateTime> MembersNotGoing { get; set; }
    }
}