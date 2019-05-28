using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meetup.Domain;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Meetup.Api
{
    public static class Queries
    {
        public static Task<MeetupDocument> Query(this IMongoDatabase db, GetMeetup queryModel) => db
            .GetCollection<MeetupDocument>("meetupevents")
            .Find<MeetupDocument>(meetup => meetup.Id == queryModel.Id)
            .FirstOrDefaultAsync();

        public class GetMeetup
        {
            public Guid Id { get; set; }
        }
        public class MeetupReadModel
        {
            public Guid Id { get; set; }

            public string Title { get; set; }

            public string Location { get; set; }

            public int NumberOfSeats { get; set; }

            public DateTime Start { get; set; }

            public DateTime End { get; set; }

            public string State { get; set; }

            public Dictionary<string, DateTime> MembersGoing { get; set; }

            public Dictionary<string, DateTime> MembersNotGoing { get; set; }
        }
    }
}