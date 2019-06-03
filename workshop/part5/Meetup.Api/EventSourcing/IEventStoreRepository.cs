using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meetup.Domain;

namespace Meetup.Api
{
    public interface IEventStoreRepository
    {
        Task<TAggregate> Get<TAggregate, TId>(TId id) where TAggregate : Aggregate<TId>;

        Task<IEnumerable<object>> GetEventStream<TAggregate, TId>(TId id) where TAggregate : Aggregate<TId>;

        Task Save<TAggregate, TId>(TAggregate entity) where TAggregate : Aggregate<TId>;
    }
}