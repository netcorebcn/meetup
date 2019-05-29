using System;
using System.Collections.Generic;
using System.Linq;

namespace Meetup.Domain
{
    public interface IProjection<TReadModel>
    {
        TReadModel Project(params object[] events);
    }

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
                    break;
                case Events.MeetupTitleUpdated ev:
                    readModel.Title = ev.Title;
                    break;
                case Events.MeetupNumberOfSeatsUpdated ev:
                    readModel.NumberOfSeats = ev.NumberOfSeats;
                    break;
                case Events.RSVPAccepted ev:
                    if (MeetupFull())
                    {
                        readModel.Waiting.Add(ev.MemberId);
                    }
                    else
                    {
                        readModel.Going.Add(ev.MemberId);
                    }
                    break;
                case Events.RSVPRejected ev:
                    readModel.NotGoing.Add(ev.MemberId);
                    break;
            }

            return readModel;

            bool MeetupFull() => readModel.NumberOfSeats == readModel.Going.Count;
        }
    }

    public class MeetupReadModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<Guid> Going { get; set; } = new List<Guid>();
        public List<Guid> NotGoing { get; set; } = new List<Guid>();
        public List<Guid> Waiting { get; set; } = new List<Guid>();
        public int NumberOfSeats { get; set; }
    }
}