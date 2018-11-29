using System;
using System.Net.Http;
using System.Threading;
using Consul;

namespace Nethium.Consul
{
    public interface IConsulHandler
    {
        CancellationToken? CancellationToken { get; set; }

        Action<ConsulClientConfiguration>? ClientConfigurationOptions { get; set; }

        Action<HttpClientHandler>? HttpClientHandlerOptions { get; set; }

        Action<HttpClient>? HttpClientOptions { get; set; }

        Func<Exception, bool>? ExceptionHandler { get; set; }

        Func<ConsulResult, bool>? BadQueryHandler { get; set; }
    }
}