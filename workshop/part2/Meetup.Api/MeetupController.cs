using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Meetup.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetupController : ControllerBase
    {
        private readonly MeetupApplicationService _appService;

        public MeetupController(MeetupApplicationService appService) => _appService = appService;

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            var meetup = await _appService.Get(id);
            return Ok(new
            {
                Id = meetup.Id.Value,
                Title = meetup.Title.Value,
                Location = meetup.Location.Value,
                NumberOfSeats = meetup.NumberOfSeats.Value,
                Start = meetup.TimeRange.Start,
                End = meetup.TimeRange.End,
                State = meetup.State
            });
        }

        [HttpPost]
        public async Task<ActionResult> Post(Meetups.V1.Create request)
        {
            await _appService.Handle(request);
            return Ok();
        }

        [HttpPut]
        [Route("title")]
        public async Task<ActionResult> Put(Meetups.V1.UpdateNumberOfSeats request)
        {
            await _appService.Handle(request);
            return Ok();
        }

        [HttpPut]
        [Route("seats")]
        public async Task<ActionResult> Put(Meetups.V1.UpdateTitle request)
        {
            await _appService.Handle(request);
            return Ok();
        }

        [HttpPut]
        [Route("location")]
        public async Task<ActionResult> Put(Meetups.V1.UpdateLocation request)
        {
            await _appService.Handle(request);
            return Ok();
        }

        [HttpPut]
        [Route("time")]
        public async Task<ActionResult> Put(Meetups.V1.UpdateDateTime request)
        {
            await _appService.Handle(request);
            return Ok();
        }

        [HttpPut]
        [Route("publish")]
        public async Task<ActionResult> Put(Meetups.V1.Publish request)
        {
            await _appService.Handle(request);
            return Ok();
        }
    }
}
