using System.Threading.Tasks;

namespace Meetup.Api
{
    public interface IMessageHandler<TMessage> : IMessageHandler where TMessage : class
    {
        Task Handle(TMessage message);
    }

    public interface IMessageHandler
    {
    }
}