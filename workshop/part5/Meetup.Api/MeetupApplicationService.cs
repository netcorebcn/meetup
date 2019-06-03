using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Meetup.Domain;

namespace Meetup.Api
{
    public class MeetupApplicationService
    {
        private readonly IEventStoreRepository _repo;

        public MeetupApplicationService(IEventStoreRepository repo) => _repo = repo;

        public Task Handle(object command) =>
            command switch
            {
                Meetups.V1.Create cmd =>
                    ExecuteCommand(
                        cmd.Id,
                        meetup => meetup.Create(MeetupId.From(cmd.Id),
                        MeetupTitle.From(cmd.Title))),
                Meetups.V1.UpdateTitle cmd =>
                    ExecuteCommand(
                        cmd.Id,
                        meetup => meetup.UpdateTitle(MeetupTitle.From(cmd.Title))),
                Meetups.V1.UpdateNumberOfSeats cmd =>
                    ExecuteCommand(
                        cmd.Id,
                        meetup => meetup.UpdateNumberOfSeats(SeatsNumber.From(cmd.NumberOfSeats))),
                Meetups.V1.UpdateLocation cmd =>
                    ExecuteCommand(
                        cmd.Id,
                        meetup => meetup.UpdateLocation(Address.From(cmd.Location))),
                Meetups.V1.UpdateDateTime cmd =>
                    ExecuteCommand(
                        cmd.Id,
                        meetup => meetup.UpdateTime(DateTimeRange.From(cmd.Start, cmd.End))),
                Meetups.V1.Publish cmd =>
                    ExecuteCommand(
                        cmd.Id,
                        meetup => meetup.Publish()),
                Meetups.V1.AcceptRSVP cmd =>
                    ExecuteCommand(
                        cmd.Id,
                        meetup => meetup.AcceptRSVP(MemberId.From(cmd.MemberId), cmd.AcceptedAt)),
                Meetups.V1.RejectRSVP cmd =>
                    ExecuteCommand(
                        cmd.Id,
                        meetup => meetup.RejectRSVP(MemberId.From(cmd.MemberId), cmd.RejectedAt)),
                Meetups.V1.Cancel cmd =>
                    ExecuteCommand(
                        cmd.Id,
                        meetup => meetup.Cancel()),
                Meetups.V1.Close cmd =>
                    ExecuteCommand(
                        cmd.Id,
                        meetup => meetup.Close()),
                _ => throw new InvalidOperationException($"Command type {command.GetType().FullName} is unknown")
            };

        private async Task ExecuteCommand(Guid id, Action<MeetupAggregate> command)
        {
            var meetup = await _repo.Get<MeetupAggregate, MeetupId>(MeetupId.From(id));
            if (meetup == null)
            {
                meetup = Aggregate<MeetupId>.Build<MeetupAggregate>();
            }

            command(meetup);
            await _repo.Save<MeetupAggregate, MeetupId>(meetup);
        }
    }
}
