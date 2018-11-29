using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nethium.DependencyInjection;
using Nethium.ServiceDiscovery;
using Nethium.Swagger;

namespace Microsoft.Extensions.DependencyInjection
{
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class NethiumExtensions
    {
        private static readonly Lazy<MethodInfo> _MakeFactoryMethod = new Lazy<MethodInfo>(
            () => typeof(NethiumExtensions)
                                .GetMethod(nameof(MakeFactory)) ??
                                    throw new InvalidOperationException()
                  );

        public static IMvcBuilder AddNethiumControllers(this IMvcBuilder builder)
        {
            var startupConfig = builder.Services.GetNethiumStartupConfig();
            var parts = from serviceRegistration in startupConfig.ServiceRegistrations
                select Assembly.GetAssembly(serviceRegistration.ImplType);
            foreach (var assembly in parts.Distinct())
            {
                builder.AddApplicationPart(assembly);
            }

            builder.Services.TryAdd(new ServiceDescriptor(typeof(NethiumStartupConfig), startupConfig));

            return builder;
        }

        public static IServiceCollection AddStub(this IServiceCollection serviceCollection,
            params Assembly[] stubAssembly)
        {
            serviceCollection.AddSingleton<ISwaggerStubResolver>(new SwaggerStubResolver(stubAssembly));
            return serviceCollection;
        }

        public static IServiceCollection AddNethiumServices(this IServiceCollection serviceCollection)
        {
            var startConfig = serviceCollection.GetNethiumStartupConfig();
            var serviceIdentifiers = serviceCollection.GetServiceIdentifiers();
            var identifiers = serviceIdentifiers.ToList();
            if (!identifiers.Any())
            {
                throw new InvalidOperationException();
            }

            var srvMap = startConfig.ServiceRegistrations.ToDictionary(sr => sr.InterfaceType);
            foreach (var srv in identifiers)
            {
                serviceCollection.RemoveAll(srv.ServiceType);
                if (srvMap.ContainsKey(srv.ServiceType))
                {
                    var sr = srvMap[srv.ServiceType];
                    serviceCollection.AddScoped(sr.InterfaceType, sr.ImplType);
                }
                else
                {
                    var makeFactory =
                        _MakeFactoryMethod.Value.MakeGenericMethod(srv.ServiceType);
                    serviceCollection.AddScoped(srv.ServiceType,
                        (Func<IServiceProvider, object>) makeFactory.Invoke(null, new object[] {srv}));
                }
            }

            return serviceCollection;
        }

        public static Func<IServiceProvider, TInterface> MakeFactory<TInterface>(IServiceIdentifier<TInterface> id)
            where TInterface : class
        {
            return provider =>
            {
                var servicesLocator = provider.GetRequiredService<IServicesLocator>();
                var startupConfig = provider.GetRequiredService<NethiumStartupConfig>();
                var serviceLocations = servicesLocator.LocateService(id,
                    startupConfig.CancellationTokenSource?.Token ?? CancellationToken.None);
                var serviceChoicer = provider.GetRequiredService<IServiceChooseStrategy<TInterface>>();
                var servicesFactory = provider.GetRequiredService<IServiceFactory<TInterface>>();
                return servicesFactory.Build(serviceChoicer.Choose(id, serviceLocations.Result));
            };
        }

        public static IServiceCollection RegisterInterface<TInterface>(this IServiceCollection serviceCollection,
            string serviceName) where TInterface : class
        {
            IServiceIdentifier<TInterface> serviceIdentifier = new ServiceIdentifier<TInterface>(serviceName);
            serviceCollection.AddSingleton(serviceIdentifier);
            return serviceCollection;
        }

        public static IServiceCollection RegisterService<TInterface, TImpl>(this IServiceCollection serviceCollection,
            string path, string checkPath, string? checkBaseUrl) where TInterface : class where TImpl : class, TInterface
        {
            var startConfig = serviceCollection.GetNethiumStartupConfig();
            var serviceIdentifier = serviceCollection.GetServiceIdentifier<TInterface>();
            serviceCollection.AddScoped<TInterface, TImpl>();
            startConfig.ServiceRegistrations.Add(
                new ServiceRegistration<TInterface, TImpl>(serviceIdentifier, path, checkPath, checkBaseUrl));
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(NethiumStartupConfig), startConfig));
            return serviceCollection;
        }

        public static IServiceCollection RegisterService<TInterface, TImpl>(this IServiceCollection serviceCollection,
            string path, string checkPath) where TInterface : class where TImpl : class, TInterface
        {
            return serviceCollection.RegisterService<TInterface, TImpl>(path, checkPath, null);
        }

        public static NethiumStartupConfig GetNethiumStartupConfig(this IServiceCollection serviceCollection)
        {
            return serviceCollection.SingleOrDefault(s => s.ServiceType == typeof(NethiumStartupConfig))
                       ?.ImplementationInstance as NethiumStartupConfig ?? new NethiumStartupConfig();
        }

        internal static IServiceIdentifier<TInterface> GetServiceIdentifier<TInterface>(
            this IServiceCollection serviceCollection)
        {
            return (IServiceIdentifier<TInterface>)
                serviceCollection
                    .Single(s =>
                        s.ServiceType == typeof(IServiceIdentifier<TInterface>) &&
                        s.ServiceType.GenericTypeArguments[0] == typeof(TInterface))
                    .ImplementationInstance;
        }

        internal static IEnumerable<IServiceIdentifier<dynamic>> GetServiceIdentifiers(
            this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .Where(s => s.Lifetime == ServiceLifetime.Singleton)
                .Where(s => s.ServiceType.IsGenericType &&
                            s.ServiceType.GetGenericTypeDefinition() == typeof(IServiceIdentifier<>))
                .Select(s => (IServiceIdentifier<dynamic>) s.ImplementationInstance);
        }
    }
}