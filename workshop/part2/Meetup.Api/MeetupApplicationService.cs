using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Meetup.Domain;

namespace Meetup.Api
{
    public class MeetupApplicationService
    {
        private readonly IMeetupRepository _repo;

        public MeetupApplicationService(IMeetupRepository repo) => _repo = repo;

        public Task<Domain.Meetup> Get(Guid id) => GetMeetup(id);

        public Task Handle(object command) =>
            command switch
            {
                Meetups.V1.Create cmd =>
                    _repo.Save(new Meetup.Domain.Meetup(
                        MeetupId.From(cmd.Id),
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
                        meetup => meetup.UpdateLocation(Address.From(cmd.Address))),
                Meetups.V1.UpdateDateTime cmd =>
                    ExecuteCommand(
                        cmd.Id,
                        meetup => meetup.UpdateTime(DateTimeRange.From(cmd.Start, cmd.End))),
                _ => throw new InvalidOperationException($"Command type {command.GetType().FullName} is unknown")
            };

        private async Task ExecuteCommand(Guid id, Action<Meetup.Domain.Meetup> command)
        {
            Domain.Meetup meetup = await GetMeetup(id);

            command(meetup);
            await _repo.Save(meetup);
        }

        private async Task<Domain.Meetup> GetMeetup(Guid id)
        {
            var meetup = await _repo.Get(id);
            if (meetup == null)
            {
                throw new Exception($"Meetup not found, id {id}");
            }

            return meetup;
        }
    }
}
