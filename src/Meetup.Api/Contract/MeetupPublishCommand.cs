using System;

namespace Meetup.Api
{
    public static class Meetup
    {
        public static class V1
        {
            public class Publish
            {
                public Guid Id { get; }
                public int NumberOfSpots { get; }

                public MeetupPublishCommand(Guid id, int numberOfSpots)
                {
                    Id = id;
                    NumberOfSpots = numberOfSpots;
                }
            }
        }
    }
}