using System;
using System.Collections.Generic;

namespace Meetup.Domain
{
    public class Meetup
    {
        public MeetupId Id { get; }
        public MeetupTitle Title { get; private set; }
        public Address Location { get; private set; }
        public SeatsNumber NumberOfSeats { get; private set; }
        public DateTimeRange TimeRange { get; private set; }

        public Meetup(MeetupId id, MeetupTitle title, Address location, SeatsNumber numberOfSeats, DateTimeRange timeRange)
        {
            Id = id;
            Title = title;
            Location = location;
            NumberOfSeats = numberOfSeats;
            TimeRange = timeRange;
        }

        public void ChangeNumberOfSeats(SeatsNumber number) => NumberOfSeats = number;
    }
}
