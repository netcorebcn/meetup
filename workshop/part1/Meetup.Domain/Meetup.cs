using System;
using System.Collections.Generic;

#nullable enable
namespace Meetup.Domain
{
    public class Meetup
    {
        public MeetupId Id { get; }
        public MeetupTitle Title { get; private set; }
        public Address Location { get; private set; }
        public SeatsNumber NumberOfSeats { get; private set; }
        public DateTimeRange TimeRange { get; private set; }

        public Meetup(MeetupId id, MeetupTitle title)
        {
            Id = id;
            Title = title;
            Location = Address.None;
            NumberOfSeats = SeatsNumber.None;
            TimeRange = DateTimeRange.None;
        }

        public void UpdateNumberOfSeats(SeatsNumber number) => NumberOfSeats = number;
        public void UpdateLocation(Address location) => Location = location;
        public void UpdateTime(DateTimeRange timeRange) => TimeRange = timeRange;
        public void UpdateTitle(MeetupTitle title) => Title = title;

        public void Publish()
        {
        }
    }
}