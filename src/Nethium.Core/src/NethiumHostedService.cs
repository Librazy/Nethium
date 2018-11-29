using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethium.Abstraction;
using Nethium.Consul;

namespace Nethium.Core
{
    public class NethiumHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly NethiumStartupConfig _startupConfig;

        public NethiumHostedService(IServiceProvider serviceProvider, NethiumStartupConfig startupConfig, IApplicationLifetime applicationLifetime)
        {
            _startupConfig = startupConfig;
            _serviceProvider = serviceProvider;
            if (startupConfig.CancellationTokenSource != null)
            {
                applicationLifetime.ApplicationStopping.Register(() => startupConfig.CancellationTokenSource?.Cancel());
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<NethiumHostedService>>();
            try
            {
                var consulServiceEndpoint = scope.ServiceProvider.GetRequiredService<IConsulServiceEndpoint>();
                var serverIdentifier = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<ServerIdentifier>>().Value;
                var port = serverIdentifier.Port ?? (serverIdentifier.BaseUrl.StartsWith("https://") ? 443 : 80);
                var canonicalBase = port != 443 && port != 80
                    ? $"{serverIdentifier.BaseUrl}:{port}"
                    : serverIdentifier.BaseUrl;
                var registerExceptions = new List<Exception>();
                logger.LogInformation($"Registering nethium service at {serverIdentifier.ServerId}");
                foreach (var sr in _startupConfig.ServiceRegistrations)
                {
                    var serviceId = $"{serverIdentifier.ServerId}-{sr.ServiceName}";
                    try
                    {
                        var result = await consulServiceEndpoint.RegisterService(new AgentServiceRegistration
                        {
                            ID = $"{serverIdentifier.ServerId}-{sr.ServiceName}",
                            Name = sr.ServiceName,
                            Address = serverIdentifier.BaseUrl,
                            Port = port,
                            Tags = new[] {$"urlprefix-{sr.Path}"},
                            Checks = new[]
                            {
                                new AgentServiceCheck
                                {
                                    HTTP = (sr.CheckBaseUrl ?? canonicalBase) + sr.CheckPath,
                                    Interval = TimeSpan.FromSeconds(10),
                                    TLSSkipVerify = true
                                }
                            }
                        }, cancellationToken);
                        logger.LogInformation($"Registered service {serviceId}: {result.StatusCode}");
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"Exception registering service {serviceId}:");
                        registerExceptions.Add(e);
                    }
                }

                if (registerExceptions.Any())
                {
                    throw new AggregateException(registerExceptions);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed executing nethium service:");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<NethiumHostedService>>();
            var serverIdentifier = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<ServerIdentifier>>().Value;
            logger.LogInformation($"Deregistering nethium service at {serverIdentifier.ServerId}");
            try
            {
                var deregisterExceptions = new List<Exception>();
                var consulServiceEndpoint = scope.ServiceProvider.GetRequiredService<IConsulServiceEndpoint>();
                foreach (var sr in _startupConfig.ServiceRegistrations)
                {
                    var serviceId = $"{serverIdentifier.ServerId}-{sr.ServiceName}";
                    try
                    {
                        var result = await consulServiceEndpoint.DeregisterService(serviceId, cancellationToken);
                        logger.LogInformation($"Deregistered service {serviceId}: {result.StatusCode}");
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"Exception registering service {serviceId}:");
                        deregisterExceptions.Add(e);
                    }
                }

                if (deregisterExceptions.Any())
                {
                    throw new AggregateException(deregisterExceptions);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed executing nethium service:");
            }
        }
    }
}