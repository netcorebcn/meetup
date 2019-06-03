using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meetup.Domain;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Meetup.Api
{
    public static class Queries
    {
        public static async Task<TReadModel> Query<TReadModel, TProjection>(this IEventStoreRepository eventStore, Guid id)
            where TProjection : IProjection<TReadModel>, new()
            where TReadModel : new()
        {
            var events = await eventStore.GetEventStream<MeetupAggregate, MeetupId>(MeetupId.From(id));
            return new TProjection().Project(new TReadModel(), events.ToArray());
        }
    }
}