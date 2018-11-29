using System;

namespace Nethium.DependencyInjection
{
    public class ServiceRegistration<TInterface, TImpl> : IServiceRegistration<TInterface, TImpl>
        where TInterface : class where TImpl : class, TInterface
    {
        public ServiceRegistration(IServiceIdentifier<TInterface> serviceIdentifier, string path, string checkPath, string? checkBaseUrl)
        {
            ServiceIdentifier = serviceIdentifier;
            Path = path;
            CheckPath = checkPath;
            CheckBaseUrl = checkBaseUrl;
        }

        public string ServiceName => ServiceIdentifier.ServiceName;

        public IServiceIdentifier<TInterface> ServiceIdentifier { get; }

        public string Path { get; }

        public string? CheckBaseUrl { get; }

        public string CheckPath { get; }

        public Type InterfaceType => typeof(TInterface);

        public Type ImplType => typeof(TImpl);
    }
}