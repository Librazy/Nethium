using System.Threading;
using System.Threading.Tasks;
using Consul;

namespace Nethium.Consul
{
    public interface IConsulServiceEndpoint
    {
        IConsulBuilder ConsulBuilder { get; }

        Task<QueryResult<ServiceEntry[]>> GetService(string service, CancellationToken ct = default);

        Task<WriteResult> RegisterService(AgentServiceRegistration serviceRegistration, CancellationToken ct = default);

        Task<WriteResult> DeregisterService(string service, CancellationToken ct = default);
    }
}