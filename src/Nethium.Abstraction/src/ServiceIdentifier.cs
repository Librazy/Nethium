using System;

namespace Nethium.DependencyInjection
{
    public class ServiceIdentifier<TInterface> : IServiceIdentifier<TInterface>
    {
        public ServiceIdentifier(string serviceName) => ServiceName = serviceName;

        public string ServiceName { get; }

        public Type ServiceType => typeof(TInterface);
    }
}