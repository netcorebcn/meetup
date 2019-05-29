using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace Meetup.Api.Controllers
{
    [Route("/meetup")]
    [ApiController]
    public class MeetupQueriesController : ControllerBase
    {
        private readonly IDocumentStore _eventStore;

        public MeetupQueriesController(IDocumentStore eventStore) => _eventStore = eventStore;

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            var result = await _eventStore.Query(new Queries.GetMeetup { Id = id });
            return Ok(result);
        }
    }
}
