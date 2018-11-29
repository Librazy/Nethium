using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Nethium.Consul;
using Nethium.DependencyInjection;

namespace Nethium.ServiceDiscovery
{
    public class ConsulServicesLocator : IServicesLocator
    {
        private readonly IConsulServiceEndpoint _consulServiceEndpoint;

        public ConsulServicesLocator(IConsulServiceEndpoint consulServiceEndpoint) =>
            _consulServiceEndpoint = consulServiceEndpoint;

        public async Task<IEnumerable<IServiceLocation<T>>> LocateService<T>(IServiceIdentifier<T> identifier,
            CancellationToken cancellationToken) where T: class
        {
            var queryResult = await _consulServiceEndpoint.GetService(identifier.ServiceName, cancellationToken);
            if (queryResult.StatusCode >= HttpStatusCode.MultipleChoices)
            {
                throw new BadConsulQueryException<QueryResult<ServiceEntry[]>>(queryResult, identifier.ServiceName);
            }

            var serviceEntries = queryResult.Response;
            return from service in serviceEntries
                select new ConsulServiceLocation<T>(identifier, service);
        }
    }
}