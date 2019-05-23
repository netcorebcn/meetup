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

        public async Task Handle(object command)
        {
            switch (command)
            {
                case Meetups.V1.Create cmd:
                    var meetup = new Meetup.Domain.Meetup(MeetupId.From(cmd.Id), MeetupTitle.From(cmd.Title));
                    await _repo.Save(meetup);
                    break;

                case Meetups.V1.UpdateTitle cmd:
                    await ExecuteCommand(
                        cmd.Id,
                        meetup => meetup.UpdateTitle(MeetupTitle.From(cmd.Title)));
                    break;

                case Meetups.V1.UpdateNumberOfSeats cmd:
                    await ExecuteCommand(
                        cmd.Id,
                        meetup => meetup.UpdateNumberOfSeats(SeatsNumber.From(cmd.NumberOfSeats)));
                    break;

                case Meetups.V1.UpdateLocation cmd:
                    await ExecuteCommand(
                        cmd.Id,
                        meetup => meetup.UpdateLocation(Address.From(cmd.Address)));
                    break;

                case Meetups.V1.UpdateDateTime cmd:
                    await ExecuteCommand(
                        cmd.Id,
                        meetup => meetup.UpdateTime(DateTimeRange.From(cmd.Start, cmd.End)));
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Command type {command.GetType().FullName} is unknown");
            }

            async Task ExecuteCommand(Guid id, Action<Meetup.Domain.Meetup> command)
            {
                var meetup = await _repo.Get(id);
                if (meetup == null)
                {
                    throw new Exception($"Meetup not found, id {id}");
                }

                command(meetup);
                await _repo.Save(meetup);
            }
        }
    }
}
