using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Meetup.Api
{
    public class MessageHandlerBuilder : IMessageHandlerBuilder
    {
        private readonly IServiceCollection _services;
        private readonly MessageHandlerRegistry _registry;

        public MessageHandlerBuilder(IServiceCollection services, MessageHandlerRegistry registry)
        {
            _services = services;
            _registry = registry;
        }

        public IMessageHandlerBuilder WithTypesFromAssemblies(params Assembly[] assemblies)
        {
            WithAsyncHandlersFromAssemblies(assemblies);
            return this;
        }

        private void WithAsyncHandlersFromAssemblies(params Assembly[] assemblies)
        {
            foreach (var subscriber in GetTypesFromAssembly(typeof(IMessageHandler<>), assemblies))
            {
                _registry.Add(subscriber.message, subscriber.wrapper);
                _services.AddTransient(subscriber.wrapper);
            }
        }

        private IEnumerable<(Type message, Type wrapper)> GetTypesFromAssembly(Type interfaceType, IEnumerable<Assembly> assemblies) =>
                from ti in assemblies.SelectMany(a => a.DefinedTypes)
                where ti.IsClass && !ti.IsAbstract && !ti.IsInterface
                from i in ti.ImplementedInterfaces
                where i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == interfaceType
                select
                (
                    messageType: i.GenericTypeArguments.First(),
                    wrapperType: ti.AsType()
                );
    }
}