using System;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nethium.Configuration;
using Nethium.Consul;

namespace Nethium.Core
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddConsul(
            this IConfigurationBuilder builder,
            string prefix,
            string separator,
            string watch,
            CancellationToken cancellationToken,
            Action<ConsulClientConfiguration> clientConfig
        )
        {
            return builder.AddConsul(
                new ConfigurationOptions
                {
                    Prefix = prefix,
                    Separator = separator,
                    Watch = watch
                },
                c =>
                {
                    c.ClientConfigurationOptions = clientConfig;
                    c.CancellationToken = cancellationToken;
                }
            );
        }

        public static IConfigurationBuilder AddConsul(
            this IConfigurationBuilder builder,
            ConfigurationOptions options,
            Action<IConsulHandler> configure
        )
        {
            var handler = new ConsulHandler();
            configure.Invoke(handler);
            return builder.AddConsul(options, handler);
        }

        public static IConfigurationBuilder AddConsul(
            this IConfigurationBuilder builder,
            ConfigurationOptions configurationOptions,
            IConsulHandler consulHandler
        ) =>
            builder.AddConsul(configurationOptions, consulHandler, (ILoggerFactory?) null);

        public static IConfigurationBuilder AddConsul(
            this IConfigurationBuilder builder,
            ConfigurationOptions configurationOptions,
            IConsulHandler consulHandler,
            ILoggerFactory? loggerFactory
        )
        {
            var consulBuilder = new ConsulBuilder(consulHandler);
            var consulEndpoint = new ConsulKvEndpoint(consulBuilder, loggerFactory.CreateLogger<ConsulKvEndpoint>())
            {
                WatchExceptionHandler = async (kv, e, key, logger, ct) =>
                {
                    await Task.Delay(10000, ct);
                    logger.LogWarning($"Restart watching {key}");
                    return true;
                }
            };
            return builder.AddConsul(configurationOptions, consulHandler, consulEndpoint);
        }

        public static IConfigurationBuilder AddConsul(
            this IConfigurationBuilder builder,
            ConfigurationOptions configurationOptions,
            IConsulHandler consulHandler,
            IConsulKvEndpoint consulKvEndpoint
        )
        {
            var consulConfigurationSource =
                new ConsulConfigurationSource(configurationOptions, consulHandler, consulKvEndpoint);
            return builder.Add(consulConfigurationSource);
        }
    }
}