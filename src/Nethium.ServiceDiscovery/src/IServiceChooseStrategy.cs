using System.Collections.Generic;
using Nethium.DependencyInjection;

namespace Nethium.ServiceDiscovery
{
    public interface IServiceChooseStrategy<TInterface> where TInterface : class
    {
        IServiceLocation<TInterface> Choose(IServiceIdentifier<TInterface> identifier, IEnumerable<IServiceLocation<TInterface>> locations);
    }
}
