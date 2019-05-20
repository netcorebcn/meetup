using System;
using System.Threading.Tasks;

namespace Meetup.Api
{
    public interface IMeetupRepository
    {
        Task<Meetup.Domain.Meetup> Get(Guid id);
        Task Save(Meetup.Domain.Meetup entity);
    }
}