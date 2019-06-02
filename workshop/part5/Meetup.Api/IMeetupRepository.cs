using System;
using System.Threading.Tasks;
using Meetup.Domain;

namespace Meetup.Api
{
    public interface IMeetupRepository
    {
        Task<MeetupAggregate> Get(Guid id);
        Task Save(MeetupAggregate entity);
    }
}