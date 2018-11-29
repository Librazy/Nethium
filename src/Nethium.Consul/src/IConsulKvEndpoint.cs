using System;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Nethium.Consul
{
    public interface IConsulKvEndpoint
    {
        IConsulBuilder ConsulBuilder { get; }

        ILogger<IConsulKvEndpoint>? Logger { get; }

        string? Watching { get; }

        Func<ConsulKvEndpoint, Exception, string, ILogger?, CancellationToken, Task<bool>>? WatchExceptionHandler
        {
            get;
            set;
        }

        Task<QueryResult<KVPair[]>> Get(string prefix, CancellationToken? cancellationToken);

        Task<WriteResult<bool>> Set(string key, string value, CancellationToken? cancellationToken);

        IChangeToken Watch(string key, CancellationToken? cancellationToken);
    }
}