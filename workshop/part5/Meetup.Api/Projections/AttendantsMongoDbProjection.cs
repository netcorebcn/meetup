using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meetup.Domain;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Meetup.Api
{
    public interface IProjection
    {
        Task Project(object @event);
    }

    public class AttendantsMongoProjection : IProjection
    {
        private ILogger<AttendantsMongoProjection> _logger;
        private Func<IMongoDatabase> _getDb;

        public AttendantsMongoProjection(Func<IMongoDatabase> getDb, ILogger<AttendantsMongoProjection> logger)
        {
            _logger = logger;
            _getDb = getDb;
        }

        public async Task Project(object @event)
        {
            try
            {
                var projection = new AttendantsProjection();
                var id = (Guid)((dynamic)@event).Id;
                var collection = _getDb().GetCollection<AttendantsReadModel>("attendants");

                var state = await collection.Find<AttendantsReadModel>(meetup => meetup.Id == id).FirstOrDefaultAsync();
                var newState = projection.Project(state ?? new AttendantsReadModel(), @event);

                _logger.LogDebug($"Projecting event {@event} with id {id}, state {JsonConvert.SerializeObject(state)} and new state {JsonConvert.SerializeObject(newState)}");
                await collection.ReplaceOneAsync(doc => doc.Id == id, newState, new UpdateOptions
                {
                    IsUpsert = true
                });
            }
            catch (Exception ex)
            {

                _logger.LogError($"Error {ex.Message}");
            }
        }
    }
}