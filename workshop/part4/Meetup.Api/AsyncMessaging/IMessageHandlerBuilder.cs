using System.Reflection;

namespace Meetup.Api
{
    public interface IMessageHandlerBuilder
    {
        IMessageHandlerBuilder WithTypesFromAssemblies(params Assembly[] assemblies);
    }
}