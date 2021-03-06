using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Meetup.Domain;

namespace Meetup.Api
{
    public static class Extensions
    {
        static Transaction WithCommand(this Transaction transaction, Func<MeetupAggregate, MeetupAggregate> command)
        {
            transaction.aggregate = command(transaction.aggregate);
            return transaction;
        }

        static Transaction WithIntegrationEvent(this Transaction transaction, Type @event)
        {
            transaction.IntegrationEvent = @event;
            return transaction;
        }

        static async Task Execute(this Transaction transaction, MeetupRepository repository)
        {
            await repository.Save(transaction.aggregate);
        }
    }

    public class Transaction
    {
        public MeetupAggregate aggregate;
        public Type IntegrationEvent;
    }
    public class MeetupApplicationService
    {
        private readonly MeetupRepository _repo;

        public MeetupApplicationService(MeetupRepository repo) => _repo = repo;

        // repo
        //     .get(id)
        //     .Command()
        //     .WithEvents()
        //     .WithIntegrationEvents()
        //     .Execute();
        public Task Handle(object command) =>
            command switch
            {
                Meetups.V1.Create cmd =>
                    _repo.Save(new MeetupAggregate(
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
            var meetup = await GetMeetup(id);
            command(meetup);
            await _repo.Save(meetup);
        }

        private async Task<MeetupAggregate> GetMeetup(Guid id)
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
