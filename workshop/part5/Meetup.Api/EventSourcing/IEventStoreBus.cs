using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Meetup.Domain;

namespace Meetup.Api
{
    public interface IEventStoreBus
    {
        Task Subscribe(Type eventType, string subscription, Func<object, Task> eventHandler);
    }
}