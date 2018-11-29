using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nethium.DependencyInjection;

namespace Nethium.ServiceDiscovery
{
    public interface IServicesLocator
    {
        Task<IEnumerable<IServiceLocation<T>>> LocateService<T>(IServiceIdentifier<T> identifier,
            CancellationToken cancellationToken) where T : class;
    }
}