using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Meetup.Api.Controllers
{
    [Route("/meetup")]
    [ApiController]
    public class MeetupQueriesController : ControllerBase
    {
        private readonly IMongoDatabase _db;

        public MeetupQueriesController(IMongoDatabase db) => _db = db;

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            var result = await _db.Query(new Queries.GetMeetup { Id = id });
            return Ok(result);
        }
    }
}
