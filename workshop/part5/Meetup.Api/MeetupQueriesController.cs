using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meetup.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Meetup.Api.Controllers
{
    [Route("/meetup")]
    [ApiController]
    public class MeetupQueriesController : ControllerBase
    {
        private readonly IEventStoreRepository _repo;

        public MeetupQueriesController(IEventStoreRepository repo) => _repo = repo;

        [HttpGet("{id}")]
        public async Task<ActionResult> GetMeetup(Guid id)
        {
            var result = await _repo.Query<MeetupReadModel, MeetupProjection>(id);
            return Ok(result);
        }

        [HttpGet("attendants/{id}")]
        public async Task<ActionResult> GetAttendants(Guid id)
        {
            var result = await _repo.Query<AttendantsReadModel, AttendantsProjection>(id);
            return Ok(result);
        }
    }
}
