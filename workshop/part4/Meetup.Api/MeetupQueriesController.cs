using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Meetup.Domain;
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
        public async Task<ActionResult> GetMeetup(Guid id)
        {
            // var result = await _eventStore.Query<MeetupReadModel, MeetupProjection>(id);
            var result = await _eventStore.Query<MeetupReadModel>(id);
            return Ok(result);
        }

        [HttpGet("attendants/{id}")]
        public async Task<ActionResult> GetAttendants(Guid id)
        {
            // var result = await _eventStore.Query<AttendantsReadModel, AttendantsProjection>(id);
            var result = await _eventStore.Query<AttendantsReadModel>(id);
            return Ok(result);
        }
    }
}
