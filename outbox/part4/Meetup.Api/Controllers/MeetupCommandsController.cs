using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Meetup.Api.Controllers
{
    [Route("/meetup")]
    [ApiController]
    public class MeetupCommandsController : ControllerBase
    {
        private readonly MeetupApplicationService _appService;

        public MeetupCommandsController(MeetupApplicationService appService) => _appService = appService;

        [HttpPost]
        public async Task<ActionResult> Post(Meetups.V1.Create request)
        {
            await _appService.Handle(request);
            return Ok();
        }

        [HttpPut]
        [Route("seats")]
        public async Task<ActionResult> Put(Meetups.V1.UpdateNumberOfSeats request)
        {
            await _appService.Handle(request);
            return Ok();
        }

        [HttpPut]
        [Route("title")]
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

        [HttpPut]
        [Route("acceptrsvp")]
        public async Task<ActionResult> Put(Meetups.V1.AcceptRSVP request)
        {
            await _appService.Handle(request);
            return Ok();
        }

        [HttpPut]
        [Route("rejectrsvp")]
        public async Task<ActionResult> Put(Meetups.V1.RejectRSVP request)
        {
            await _appService.Handle(request);
            return Ok();
        }

        [HttpPut]
        [Route("cancel")]
        public async Task<ActionResult> Put(Meetups.V1.Cancel request)
        {
            await _appService.Handle(request);
            return Ok();
        }

        [HttpPut]
        [Route("close")]
        public async Task<ActionResult> Put(Meetups.V1.Close request)
        {
            await _appService.Handle(request);
            return Ok();
        }
    }
}
