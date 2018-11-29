using System.Collections.Generic;
using Nethium.DependencyInjection;

namespace Nethium.ServiceDiscovery
{
    public interface IServiceLocation<out TInterface> where TInterface : class
    {
        IServiceIdentifier<TInterface> Service { get; }

        string?[] Tags { get; }

        int? Port { get; }

        string? Address { get; }

        IDictionary<string, string> Meta { get; }
    }
}