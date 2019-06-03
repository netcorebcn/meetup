using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meetup.Domain;
using MongoDB.Driver;

namespace Meetup.Api
{
    public interface IProjection
    {
        Task Project(object @event);
    }

    public class AttendantsMongoProjection : IProjection
    {
        private Func<IMongoDatabase> _getDb;


        public AttendantsMongoProjection(Func<IMongoDatabase> getDb) => _getDb = getDb;

        public async Task Project(object @event)
        {
            var projection = new AttendantsProjection();
            var id = (Guid)((dynamic)@event).Id;
            var collection = _getDb().GetCollection<AttendantsReadModel>("attendants");

            var state = await collection.Find<AttendantsReadModel>(meetup => meetup.Id == id).FirstOrDefaultAsync();
            var newState = projection.Project(state, @event);

            await collection.ReplaceOneAsync(doc => doc.Id == id, newState, new UpdateOptions
            {
                IsUpsert = true
            });
        }
    }
}