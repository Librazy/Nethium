using System;
using Consul;

namespace Nethium.Consul
{
    public class ConsulBuilder : IConsulBuilder
    {
        public ConsulBuilder(IConsulHandler consulHandler) => ConsulHandler = consulHandler;

        public IConsulHandler ConsulHandler { get; }

        public Action<IConsulClient>? ClientOptions { get; set; }

        public IConsulClient Build()
        {
            var client = new ConsulClient(
                ConsulHandler.ClientConfigurationOptions,
                ConsulHandler.HttpClientOptions,
                ConsulHandler.HttpClientHandlerOptions);

            ClientOptions?.Invoke(client);

            return client;
        }
    }
}