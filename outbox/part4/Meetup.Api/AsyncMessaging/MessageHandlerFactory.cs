using System;
using Microsoft.Extensions.DependencyInjection;

namespace Meetup.Api
{
    public class MessageHandlerFactory
    {
        private readonly IServiceCollection _services;

        public MessageHandlerFactory(IServiceCollection services) => _services = services;

        public IServiceProvider CreateBuilder() => _services.BuildServiceProvider();
    }
}