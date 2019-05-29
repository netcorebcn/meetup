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
                    Going(ev.MemberId);
                    break;
                case Events.RSVPRejected ev:
                    NotGoing(ev.MemberId);
                    UpdateWaitingList();
                    break;
            }

            return readModel;

            bool MeetupFull() => readModel.NumberOfSeats == readModel.Going.Count;

            void Going(Guid member)
            {
                if (MeetupFull())
                {
                    readModel.Waiting.Add(member);
                    readModel.Going.Remove(member);
                    readModel.NotGoing.Remove(member);
                }
                else
                {
                    readModel.Going.Add(member);
                    readModel.NotGoing.Remove(member);
                    readModel.Waiting.Remove(member);
                }
            }

            void NotGoing(Guid member)
            {
                readModel.NotGoing.Add(member);
                readModel.Going.Remove(member);
                readModel.Waiting.Remove(member);
            }

            void UpdateWaitingList()
            {
                if (!MeetupFull())
                {
                    var firstWaiting = readModel.Waiting.FirstOrDefault();
                    if (firstWaiting != default)
                    {
                        readModel.Waiting.Remove(firstWaiting);
                        readModel.Going.Add(firstWaiting);
                    }
                }
            }
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