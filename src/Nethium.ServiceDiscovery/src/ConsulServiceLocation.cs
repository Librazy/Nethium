using System.Collections.Generic;
using Consul;
using Nethium.DependencyInjection;

namespace Nethium.ServiceDiscovery
{
    public class ConsulServiceLocation<TInterface> : IServiceLocation<TInterface> where TInterface : class
    {
        public ConsulServiceLocation(IServiceIdentifier<TInterface> service, ServiceEntry serviceEntry)
        {
            var agentService = serviceEntry.Service;
            Service = service;
            ServiceEntry = serviceEntry;
            AgentService = agentService;
            Tags = agentService.Tags;
            Port = agentService.Port;
            Address = agentService.Address;
            Meta = agentService.Meta;
        }

        public AgentService AgentService { get; }

        public IServiceIdentifier<TInterface> Service { get; }

        public ServiceEntry ServiceEntry { get; }

        public string?[] Tags { get; }

        public int? Port { get; }

        public string? Address { get; }

        public IDictionary<string, string> Meta { get; }
    }
}