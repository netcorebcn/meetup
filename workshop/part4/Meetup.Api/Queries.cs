using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meetup.Domain;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Marten;

namespace Meetup.Api
{
    public static class Queries
    {
        public static async Task<MeetupReadModel> Query(this IDocumentStore eventStore, GetMeetup queryModel)
        {
            using var session = eventStore.OpenSession();
            var stream = await session.Events.FetchStreamAsync(queryModel.Id);
            return new MeetupProjection().Project(stream.Select(@event => @event.Data).ToArray());
        }

        public class GetMeetup
        {
            public Guid Id { get; set; }
        }
    }
}