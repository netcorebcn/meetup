using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Meetup.Domain.Commands;

namespace Meetup.Api
{
    [Route("api/[controller]")]
    public class MeetupController : Controller
    {
        private readonly MeetupApplicationService _appService;

        public MeetupController(MeetupApplicationService appService) => _appService = appService;

        [HttpGet("{id}")]
        public object Get(Guid id) => _appService.GetState(id);

        [HttpPost]
        public string Publish([FromBody]MeetupPublishCommand command) => _appService.Publish(command);

        [HttpPut("{id}")]
        public string Close(Guid id, [FromBody]string value) => _appService.Close(id);

        [HttpDelete("{id}")]
        public string Cancel(Guid id) => _appService.Cancel(id);
    }
}
