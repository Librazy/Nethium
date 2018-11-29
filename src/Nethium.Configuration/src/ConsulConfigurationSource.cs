using Microsoft.Extensions.Configuration;
using Nethium.Consul;

namespace Nethium.Configuration
{
    public class ConsulConfigurationSource : IConfigurationSource
    {
        public ConsulConfigurationSource(ConfigurationOptions options, IConsulHandler consulHandler,
            IConsulKvEndpoint consulKvEndpoint)
        {
            Options = options;
            ConsulKvEndpoint = consulKvEndpoint;
            ConsulHandler = consulHandler;
        }

        public ConfigurationOptions Options { get; }

        public IConsulKvEndpoint ConsulKvEndpoint { get; }

        public IConsulHandler ConsulHandler { get; }

        public IConfigurationProvider Build(IConfigurationBuilder builder) =>
            new ConsulConfigurationProvider(Options, ConsulHandler, ConsulKvEndpoint);
    }
}