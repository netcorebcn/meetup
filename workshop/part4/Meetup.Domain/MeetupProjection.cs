using System;
using System.Collections.Generic;
using System.Linq;

namespace Meetup.Domain
{
    public class MeetupProjection : IProjection<MeetupReadModel>
    {
        private readonly MeetupReadModel readModel = new MeetupReadModel();

        public MeetupReadModel Project(params object[] events) =>
            events.Aggregate(readModel, When);

        private MeetupReadModel When(MeetupReadModel readModel, object @event)
        {
            switch (@event)
            {
                case Events.MeetupCreated ev:
                    readModel.Id = ev.Id;
                    readModel.Title = ev.Title;
                    readModel.State = "Created";
                    break;
                case Events.MeetupPublished ev:
                    readModel.State = "Published";
                    break;
                case Events.MeetupCanceled ev:
                    readModel.State = "Canceled";
                    break;
                case Events.MeetupClosed ev:
                    readModel.State = "Closed";
                    break;
                case Events.MeetupNumberOfSeatsUpdated ev:
                    readModel.NumberOfSeats = ev.NumberOfSeats;
                    break;
                case Events.MeetupTimeUpdated ev:
                    readModel.Start = ev.Start;
                    readModel.End = ev.End;
                    break;
                case Events.MeetupTitleUpdated ev:
                    readModel.Title = ev.Title;
                    break;
                case Events.MeetupLocationUpdated ev:
                    readModel.Location = ev.Location;
                    break;
                case Events.RSVPAccepted ev:
                    readModel.MembersGoing.Add(ev.MemberId.ToString(), ev.AcceptedAt);
                    break;
                case Events.RSVPRejected ev:
                    readModel.MembersNotGoing.Add(ev.MemberId.ToString(), ev.RejectedAt);
                    break;
            }

            return readModel;
        }
    }

    public class MeetupReadModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Location { get; set; }

        public int NumberOfSeats { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string State { get; set; }

        public Dictionary<string, DateTime> MembersGoing { get; set; } = new Dictionary<string, DateTime>();

        public Dictionary<string, DateTime> MembersNotGoing { get; set; } = new Dictionary<string, DateTime>();
    }
}