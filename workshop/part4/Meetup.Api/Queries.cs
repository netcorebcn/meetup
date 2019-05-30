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
        public static async Task<TReadModel> Query<TReadModel, TProjection>(this IDocumentStore eventStore, Guid id)
            where TProjection : IProjection<TReadModel>, new()
        {
            using var session = eventStore.OpenSession();
            var stream = await session.Events.FetchStreamAsync(id);
            return new TProjection().Project(stream.Select(@event => @event.Data).ToArray());
        }
    }
}