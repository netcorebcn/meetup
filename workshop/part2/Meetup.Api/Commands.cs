using System;

namespace Meetup.Api
{
    public static class Meetups
    {
        public static class V1
        {
            public class Create
            {
                public Guid Id { get; set; }
                public string Title { get; set; }
            }

            public class UpdateNumberOfSeats
            {
                public Guid Id { get; set; }
                public int NumberOfSeats { get; set; }
            }

            public class Publish
            {
                public Guid Id { get; set; }
            }

            public class UpdateDateTime
            {
                public Guid Id { get; set; }
                public DateTime Start { get; set; }
                public DateTime End { get; set; }
            }

            public class UpdateLocation
            {
                public Guid Id { get; set; }
                public string Location { get; set; }
            }

            public class UpdateTitle
            {
                public Guid Id { get; set; }
                public string Title { get; set; }
            }
        }
    }
}