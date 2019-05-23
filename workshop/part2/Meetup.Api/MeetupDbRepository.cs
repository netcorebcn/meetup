using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meetup.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Meetup.Api
{
    public class MongoDbVotingService : IMeetupRepository
    {
        private readonly IMongoCollection<MeetupDocument> _meetups;

        public MongoDbVotingService(string connectionString)
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase("meetup");
            _meetups = db.GetCollection<MeetupDocument>("meetupevents");
        }

        public async Task<Domain.Meetup> Get(Guid id)
        {
            var doc = await _meetups.Find<MeetupDocument>(meetup => meetup.Id == id.ToString()).FirstOrDefaultAsync();

            return new Domain.Meetup(
                MeetupId.From(id),
                MeetupTitle.From(doc.Title),
                Address.From(doc.Location),
                SeatsNumber.From(doc.NumberOfSeats),
                DateTimeRange.From(doc.Start, doc.End),
                Enum.Parse<MeetupState>(doc.State));
        }

        public async Task Save(Domain.Meetup entity)
        {
            await _meetups.ReplaceOneAsync(_ => true, new MeetupDocument
            {
                Id = entity.Id,
                Title = entity.Title,
                Location = entity.Location,
                NumberOfSeats = entity.NumberOfSeats,
                Start = entity.TimeRange.Start,
                End = entity.TimeRange.End,
                State = entity.State.ToString()
            },
            new UpdateOptions
            {
                IsUpsert = true
            });
        }
    }

    public class MeetupDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

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
    }
}