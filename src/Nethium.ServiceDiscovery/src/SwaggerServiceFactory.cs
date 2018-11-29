using System;
using System.Net.Http;
using Nethium.Swagger;

namespace Nethium.ServiceDiscovery
{
    public class SwaggerServiceFactory<TInterface> : IServiceFactory<TInterface> where TInterface : class
    {
        private readonly IHttpClientFactory _factory;
        private readonly ISwaggerStubHandler _stubHandler;
        private readonly ISwaggerStubResolver _stubResolver;

        public SwaggerServiceFactory(ISwaggerStubResolver stubResolver, ISwaggerStubHandler stubHandler,
            IHttpClientFactory factory)
        {
            _stubResolver = stubResolver;
            _stubHandler = stubHandler;
            _factory = factory;
        }

        public TInterface Build(IServiceLocation<TInterface> location)
        {
            var resolver = _stubResolver;
            var handler = _stubHandler
                .Address(location.Address!)
                .Port(location.Port)
                .Meta(location.Meta!)
                .Tags(location.Tags!);

            var stub = resolver.Resolve<TInterface>();
            var instance = (TInterface) Activator.CreateInstance(stub,
                $"{location.Address!.TrimEnd('/')!}{(location.Port == null ? ":" + location.Port : "")}",
                _factory.CreateClient());

            if (!(instance is IStub))
            {
                throw new ArgumentException();
            }

            ((IStub) instance).UseHandler(handler);

            return instance;
        }
    }
}