using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Nethium.Abstraction;
using Nethium.Authentication;
using Nethium.Configuration;
using Nethium.Consul;
using Nethium.ServiceDiscovery;
using Nethium.Swagger;

namespace Nethium.Core
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class NethiumConfig
    {
        public bool HostConfiguration { get; set; }

        public string? Prefix { get; set; }

        public string? ConfigurationPrefix { get; set; }

        public string? Separator { get; set; }

        public string? Watch { get; set; }

        public bool? AutoReload { get; set; }

        public ILoggerFactory? LoggerFactory { get; set; }

        public CancellationToken? CancellationToken { get; set; }

        public IConfiguration? BootstrapConfig { get; set; }

        public Action<ConsulClientConfiguration>? ClientConfig { get; set; }

        public void UseDefaultBootstrapConfig(string[]? args = null)
        {
            BootstrapConfig = new ConfigurationBuilder()
                .AddJsonFile("nethium.json", true)
                .AddCommandLine(args ?? new string[] { })
                .AddEnvironmentVariables("NETHIUM_")
                .Build();
        }
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseNethium(this IWebHostBuilder builder)
        {
            return builder.UseNethium(o => o.UseDefaultBootstrapConfig());
        }

        public static IWebHostBuilder UseNethium(this IWebHostBuilder builder, string[] args)
        {
            return builder.UseNethium(o => o.UseDefaultBootstrapConfig(args));
        }

        public static IWebHostBuilder UseNethium(this IWebHostBuilder builder, string[]? args, ILoggerFactory logger)
        {
            return builder.UseNethium(o =>
            {
                o.UseDefaultBootstrapConfig(args);
                o.LoggerFactory = logger;
            });
        }

        public static IWebHostBuilder UseNethium(this IWebHostBuilder builder, string[]? args, ILoggerFactory logger,
            CancellationToken cancellationToken)
        {
            return builder.UseNethium(o =>
            {
                o.UseDefaultBootstrapConfig(args);
                o.LoggerFactory = logger;
                o.CancellationToken = cancellationToken;
            });
        }

        [SuppressMessage("ReSharper", "NotResolvedInText")]
        public static IWebHostBuilder UseNethium(this IWebHostBuilder builder,
            Action<NethiumConfig> configureNethiumOption)
        {
            var nethiumConfig = new NethiumConfig();
            configureNethiumOption(nethiumConfig);
            nethiumConfig.CancellationToken ??= builder.GetShutdownToken();
            var bootstrapConfig = nethiumConfig.BootstrapConfig;
            var logger = nethiumConfig.LoggerFactory?.CreateLogger<NethiumConfig>();

            if (bootstrapConfig != null)
            {
                nethiumConfig.Prefix ??= bootstrapConfig["Nethium:Consul:Prefix"] ?? throw new ArgumentNullException("Nethium:Consul:Prefix", "Consul:Prefix not specified in config");
                nethiumConfig.ConfigurationPrefix ??= bootstrapConfig["Nethium:Consul:ConfigurationPrefix"] ??
                                                      nethiumConfig.Prefix;
                nethiumConfig.Separator ??= bootstrapConfig["Nethium:Consul:Separator"];
                nethiumConfig.Watch ??= bootstrapConfig["Nethium:Consul:Watch"];
            }

            var configurationOptions = new ConfigurationOptions
            {
                Prefix = nethiumConfig.Prefix!,
                ConfigurationPrefix = nethiumConfig.ConfigurationPrefix,
                Separator = nethiumConfig.Separator!,
                Watch = nethiumConfig.Watch,
                AutoReload = nethiumConfig.AutoReload ?? false
            };

            var consulHandler = new ConsulHandler
            {
                ClientConfigurationOptions = ClientConfiguration(bootstrapConfig, nethiumConfig),
                CancellationToken = nethiumConfig.CancellationToken
            };
            var consulBuilder = new ConsulBuilder(consulHandler);
            var consulClientConfiguration = new ConsulClientConfiguration();
            consulHandler.ClientConfigurationOptions.Invoke(consulClientConfiguration);
            foreach (var pair in bootstrapConfig.AsEnumerable())
            {
                logger?.LogInformation(pair.Key + ": " + pair.Value);
            }
            logger?.LogInformation(bootstrapConfig?["Nethium:Consul:Watch"]);
            logger?.LogInformation($"Using consul at address {consulClientConfiguration.Address} " +
                                   $"{(string.IsNullOrEmpty(consulClientConfiguration.Datacenter) ? "" : "datacenter " + consulClientConfiguration.Datacenter)}");
            logger?.LogInformation($"Consul config prefix {configurationOptions.Prefix}" +
                                   $"{(string.IsNullOrEmpty(configurationOptions.ConfigurationPrefix) ? "" : " (" + configurationOptions.ConfigurationPrefix + ") ")}" +
                                   $"{(string.IsNullOrEmpty(configurationOptions.Watch) ? "" : "watching " + configurationOptions.Watch + (configurationOptions.AutoReload ? " auto-reload" : " no auto-reload"))}");
            if (nethiumConfig.HostConfiguration)
            {
                logger?.LogInformation("Injecting Nethium config provider to host configuration");
                var consulKvEndpoint = new ConsulKvEndpoint(consulBuilder,
                    nethiumConfig.LoggerFactory?.CreateLogger<ConsulKvEndpoint>())
                {
                    WatchExceptionHandler = WatchExceptionHandler
                };
                var configurationBuilder = new ConfigurationBuilder();
                if (bootstrapConfig != null)
                {
                    configurationBuilder.AddConfiguration(bootstrapConfig);
                }

                var configurationRoot = configurationBuilder
                    .AddConsul(configurationOptions, consulHandler, consulKvEndpoint)
                    .Build();

                builder
                    .UseConfiguration(configurationRoot);
            }

            builder
                .ConfigureServices(ConfigureServices(nethiumConfig, consulHandler, consulBuilder))
                .ConfigureAppConfiguration(ConfigureAppConfiguration(configurationOptions, nethiumConfig, consulHandler,
                    consulBuilder));
            return builder;
        }

        private static CancellationToken GetShutdownToken(this IWebHostBuilder builder)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            builder.ConfigureServices(s =>
            {
                var config = s.GetNethiumStartupConfig();
                config.CancellationTokenSource = cancellationTokenSource;
            });
            return cancellationTokenSource.Token;
        }

        private static TimeSpan ToTimeSpanSeconds(this string span) => TimeSpan.FromSeconds(Convert.ToDouble(span));

        private static async Task<bool> WatchExceptionHandler(ConsulKvEndpoint kv, Exception e, string key,
            ILogger? logger, CancellationToken ct)
        {
            await Task.Delay(10000, ct);
            logger?.LogWarning($"Restart watching {key}");
            return true;
        }

        [SuppressMessage("ReSharper", "NotResolvedInText")]
        private static Action<WebHostBuilderContext, IServiceCollection> ConfigureServices(
            NethiumConfig nethiumConfig, IConsulHandler consulHandler, IConsulBuilder consulBuilder
        ) =>
            (context, collection) =>
            {
                var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(context.Configuration["Nethium:ServerSecretKey"] ?? throw new ArgumentNullException("Nethium:ServerSecretKey", "ServerSecretKey not specified in config")));

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ValidateAudience = false,
                    ValidateActor = false,
                    ValidateIssuer = false
                };

                collection.AddSingleton(
                    new JwtHeader(new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)));

                collection.AddHttpContextAccessor();
                collection.AddHttpClient();
                collection.Configure<ServerIdentifier>(context.Configuration.GetSection("Nethium"));
                collection.Configure<ConfigurationOptions>(o =>
                {
                    o.Prefix = nethiumConfig.Prefix!;
                    o.ConfigurationPrefix = nethiumConfig.ConfigurationPrefix;
                    o.Separator = nethiumConfig.Separator!;
                    o.Watch = nethiumConfig.Watch;
                    o.AutoReload = nethiumConfig.AutoReload ?? false;
                });
                collection.AddSingleton(consulHandler);
                collection.AddSingleton(consulBuilder);
                collection.AddScoped<IAuthHandler, StubAuthHandler>();
                collection.AddScoped<ISwaggerStubHandler, SwaggerStubHandler>();
                collection.AddScoped<IConsulKvEndpoint, ConsulKvEndpoint>();
                collection.AddScoped<IConsulServiceEndpoint, ConsulServiceEndpoint>();
                collection.AddScoped<IServicesLocator, ConsulServicesLocator>();
                collection.AddScoped(typeof(IServiceFactory<>), typeof(SwaggerServiceFactory<>));
                collection.AddScoped(typeof(IServiceChooseStrategy<>), typeof(NearestStrategy<>));
                collection.AddHostedService<NethiumHostedService>();
                collection.AddOpenApiDocument();
                collection
                    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options => { options.TokenValidationParameters = tokenValidationParameters; });
            };

        private static Action<WebHostBuilderContext, IConfigurationBuilder> ConfigureAppConfiguration(
            ConfigurationOptions configurationOptions,
            NethiumConfig nethiumConfig,
            IConsulHandler consulHandler,
            IConsulBuilder consulBuilder
        ) =>
            (context, configurationBuilder) =>
            {
                var consulKvEndpoint = new ConsulKvEndpoint(consulBuilder,
                    nethiumConfig.LoggerFactory?.CreateLogger<ConsulKvEndpoint>())
                {
                    WatchExceptionHandler = WatchExceptionHandler
                };
                configurationBuilder
                    .AddJsonFile("nethium.json", true, true)
                    .AddEnvironmentVariables("NETHIUM_")
                    .Add(new ConsulConfigurationSource(configurationOptions, consulHandler, consulKvEndpoint));
            };

        private static Action<ConsulClientConfiguration> ClientConfiguration(
            IConfiguration? bootstrapConfig, NethiumConfig nethiumConfig
        ) =>
            config =>
            {
                if (bootstrapConfig != null)
                {
                    config.Address = bootstrapConfig["Nethium:Consul:Address"] != null
                        ? new Uri(bootstrapConfig["Nethium:Consul:Address"])
                        : config.Address;
                    config.Datacenter = bootstrapConfig["Nethium:Consul:Datacenter"];
                    config.Token = bootstrapConfig["Nethium:Consul:Token"];
                    config.WaitTime = bootstrapConfig["Nethium:Consul:WaitTime"]?.ToTimeSpanSeconds();
                }

                nethiumConfig.ClientConfig?.Invoke(config);
            };
    }
}