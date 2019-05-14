using System;

namespace Meetup.Domain.Commands
{
    public class MeetupPublishCommand: MeetupCommand
    {
        public int NumberOfSpots { get; }

        public MeetupPublishCommand(Guid meetupId, int numberOfSpots) : base(meetupId)
        {
            NumberOfSpots = numberOfSpots;
        }
    }
}