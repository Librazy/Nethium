using System.Collections.Generic;
using System.Linq;
using Nethium.DependencyInjection;

namespace Nethium.ServiceDiscovery
{
    public class NearestStrategy<TInterface> : IServiceChooseStrategy<TInterface> where TInterface: class
    {
        public IServiceLocation<TInterface> Choose(IServiceIdentifier<TInterface> identifier, IEnumerable<IServiceLocation<TInterface>> locations)
            => locations.First();
    }
}
