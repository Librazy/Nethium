using System;
using System.Net.Http;
using System.Threading;
using Consul;

namespace Nethium.Consul
{
    public class ConsulHandler : IConsulHandler
    {
        public CancellationToken? CancellationToken { get; set; }

        public Action<ConsulClientConfiguration>? ClientConfigurationOptions { get; set; }

        public Action<HttpClientHandler>? HttpClientHandlerOptions { get; set; }

        public Action<HttpClient>? HttpClientOptions { get; set; }

        public Func<Exception, bool>? ExceptionHandler { get; set; } = exception => false;

        public Func<ConsulResult, bool>? BadQueryHandler { get; set; } = result => false;
    }
}