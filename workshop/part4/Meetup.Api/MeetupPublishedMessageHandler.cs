using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Meetup.Domain;

namespace Meetup.Api
{
    public class MeetupPublishedMessageHandler : IMessageHandler<Events.MeetupPublished>
    {
        private ILogger<MeetupPublishedMessageHandler> _logger;

        public MeetupPublishedMessageHandler(ILogger<MeetupPublishedMessageHandler> logger) => _logger = logger;

        public Task Handle(Events.MeetupPublished message)
        {
            _logger.LogInformation($"SENDING EMAIL FOR Meetup {message.Id}");
            return Task.CompletedTask;
        }
    }
}