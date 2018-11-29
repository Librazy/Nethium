using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethium.Abstraction;

namespace Nethium.Consul
{
    public class ConsulServiceEndpoint : IConsulServiceEndpoint
    {
        public ConsulServiceEndpoint(IConsulBuilder consulBuilder, IOptionsSnapshot<ServerIdentifier> server,
            ILogger<ConsulServiceEndpoint> logger)
        {
            ConsulBuilder = consulBuilder;
            Logger = logger;
            ServerIdentifier = server.Value;
        }

        private readonly Lazy<QueryOptions> _queryOptions = new Lazy<QueryOptions>(() => new QueryOptions() {Near = "_agent"});

        public ILogger<ConsulServiceEndpoint> Logger { get; }

        public IServerIdentifier ServerIdentifier { get; }

        public IConsulBuilder ConsulBuilder { get; }

        public async Task<QueryResult<ServiceEntry[]>> GetService(string service, CancellationToken ct = default)
        {
            using var consul = ConsulBuilder.Build();
            return await consul.Health.Service(service, null, true, _queryOptions.Value, ct);
        }

        public async Task<WriteResult> RegisterService(AgentServiceRegistration serviceRegistration,
            CancellationToken ct = default)
        {
            var serverTag = serviceRegistration.Tags.SingleOrDefault(t => t.StartsWith("server-"));
            if (serverTag == null)
            {
                serverTag = "server-" + ServerIdentifier.ServerId;
                serviceRegistration.Tags = serviceRegistration.Tags.Concat(new[] {serverTag}).ToArray();
            }

            if (serverTag != "server-" + ServerIdentifier.ServerId)
            {
                throw new ArgumentException("Wrong 'server-' tag specified", nameof(serviceRegistration));
            }

            Logger?.LogInformation(
                $"Registering service {serviceRegistration.Name} ({serviceRegistration.ID}) with tags [{string.Join(", ", serviceRegistration.Tags)}]");
            using var consul = ConsulBuilder.Build();
            return await consul.Agent.ServiceRegister(serviceRegistration, ct);
        }

        public async Task<WriteResult> DeregisterService(string service, CancellationToken ct = default)
        {
            Logger?.LogInformation($"Deregistering service {service}]");
            using var consul = ConsulBuilder.Build();
            return await consul.Agent.ServiceDeregister(service, ct);
        }
    }
}