using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Nethium.Consul;

namespace Nethium.Configuration
{
    public class ConsulConfigurationProvider : ConfigurationProvider
    {
        private readonly IConsulHandler _consulHandler;
        private readonly IConsulKvEndpoint _consulKvEndpoint;
        private readonly ConfigurationOptions _options;

        public ConsulConfigurationProvider(ConfigurationOptions options, IConsulHandler consulHandler,
            IConsulKvEndpoint consulKvEndpoint)
        {
            _options = options;
            _consulKvEndpoint = consulKvEndpoint;
            _consulHandler = consulHandler;
            AutoReload = _options.AutoReload;
            if (options.Watch != null)
            {
                ChangeToken.OnChange(
                    () => _consulKvEndpoint.Watch(_options.Watch!, _consulHandler.CancellationToken),
                    async () =>
                    {
                        if (!AutoReload)
                        {
                            return;
                        }

                        await LoadAsync();
                        OnReload();
                    });
            }
        }

        public bool AutoReload { get; set; }

        public override void Load()
        {
            LoadAsync(true).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private async Task LoadAsync(bool force = false)
        {
            try
            {
                var result = await _consulKvEndpoint
                    .Get(_options.Prefix, _consulHandler.CancellationToken)
                    .ConfigureAwait(false);
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    if (!(_consulHandler.BadQueryHandler?.Invoke(result) ?? false) || force)
                    {
                        throw new BadConsulQueryException<QueryResult<KVPair[]>>(result, _options.Prefix);
                    }
                }

                var configurationPrefix = _options.ConfigurationPrefix ?? _options.Prefix;

                Data = (result.Response ?? new KVPair[0])
                    .Where(kvp =>
                        kvp.Key.StartsWith(configurationPrefix)
                        && !(_options.Separator != null && kvp.Key.Remove(0, configurationPrefix.Length).Contains(_options.Separator))
                    )
                    .ToDictionary(kvp => kvp.Key.Remove(0, configurationPrefix.Length),
                        kvp => kvp.Value == null ? null : Encoding.UTF8.GetString(kvp.Value),
                        StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception exception)
            {
                if (!(_consulHandler.ExceptionHandler?.Invoke(exception) ?? false) || force)
                {
                    throw;
                }
            }
        }

        public async Task<bool> SetAsync(string key, string value)
        {
            var configurationPrefix = _options.ConfigurationPrefix ?? _options.Prefix;

            var result = await _consulKvEndpoint
                .Set(configurationPrefix + key, value, _consulHandler.CancellationToken)
                .ConfigureAwait(false);

            if (result.StatusCode != HttpStatusCode.OK)
            {
                if (!(_consulHandler.BadQueryHandler?.Invoke(result) ?? false))
                {
                    throw new BadConsulQueryException<WriteResult<bool>>(result, _options.Prefix);
                }
            }

            base.Set(key, value);
            return result.Response;
        }

        public override void Set(string key, string value)
        {
            SetAsync(key, value).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}