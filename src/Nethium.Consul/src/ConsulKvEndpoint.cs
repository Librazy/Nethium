using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Nethium.Consul
{
    public class ConsulKvEndpoint : IConsulKvEndpoint
    {
        private ulong _lastIndex;

        private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();

        public ConsulKvEndpoint(IConsulBuilder consulBuilder, ILogger<ConsulKvEndpoint>? logger)
        {
            ConsulBuilder = consulBuilder;
            Logger = logger;
        }

        public Task? Watcher { get; private set; }

        public IConsulBuilder ConsulBuilder { get; }

        public ILogger<IConsulKvEndpoint>? Logger { get; }

        public string? Watching { get; private set; }

        public Func<ConsulKvEndpoint, Exception, string, ILogger?, CancellationToken, Task<bool>>? WatchExceptionHandler
        {
            get;
            set;
        }

        public Task<QueryResult<KVPair[]>> Get(string prefix, CancellationToken? cancellationToken) =>
            Query(prefix, cancellationToken ?? CancellationToken.None);

        public async Task<WriteResult<bool>> Set(string key, string value, CancellationToken? cancellationToken)
        {
            using var consul = ConsulBuilder.Build();
            return await consul.KV.Put(new KVPair(key) {Value = Encoding.UTF8.GetBytes(value)},
                cancellationToken ?? CancellationToken.None);
        }

        public IChangeToken Watch(string key, CancellationToken? cancellationToken)
        {
            if (Watcher != null)
            {
                return _reloadToken;
            }

            Watcher = Poll(key, cancellationToken ?? CancellationToken.None);
            Logger?.LogInformation($"Start watching {key}");
            Watching = key;
            return _reloadToken;
        }

        private async Task<bool> HasValueChanged(string key, CancellationToken cancellationToken)
        {
            QueryOptions queryOptions;
            lock (this)
            {
                queryOptions = new QueryOptions {WaitIndex = _lastIndex};
            }

            var result = await Query(key, cancellationToken, queryOptions).ConfigureAwait(false);
            return result != null && IfChanged(result);
        }

        private async Task Poll(
            string key,
            CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (!await HasValueChanged(key, cancellationToken).ConfigureAwait(false))
                        {
                            Logger?.LogDebug($"False waken watching {key}");
                            continue;
                        }

                        Logger?.LogInformation($"Key {key} changed");
                        var previousToken = Interlocked.Exchange(
                            ref _reloadToken,
                            new ConfigurationReloadToken());
                        previousToken.OnReload();
                    }
                    catch (Exception exception)
                    {
                        Watching = null;
                        Logger?.LogWarning(exception, $"Exception watching {key}:");
                        if (!await (WatchExceptionHandler?.Invoke(this, exception, key, Logger, cancellationToken) ??
                                    Task.FromResult(false)))
                        {
                            throw;
                        }
                    }
                }
            }
            finally
            {
                Watcher = null;
            }
        }

        private bool IfChanged(QueryResult queryResult)
        {
            lock (this)
            {
                if (queryResult.LastIndex <= _lastIndex)
                {
                    return false;
                }

                _lastIndex = queryResult.LastIndex;
                return true;
            }
        }


        private async Task<QueryResult<KVPair[]>> Query(
            string prefix,
            CancellationToken cancellationToken,
            QueryOptions? queryOptions = null)
        {
            using var consul = ConsulBuilder.Build();
            var result =
                await consul.KV.List(prefix, queryOptions, cancellationToken).ConfigureAwait(false);

            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.NotFound:
                    return result;
                default:
                    throw new BadConsulQueryException<QueryResult<KVPair[]>>(result, prefix,
                        queryOptions == null
                            ? null
                            : new Dictionary<string, object> {{"QueryOptions", queryOptions}});
            }
        }
    }
}