using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nethium.Demo.Abstraction;
using Newtonsoft.Json;

namespace Nethium.Demo.Stub
{
#pragma warning disable
    [GeneratedCode("NSwag", "12.1.0.0 (NJsonSchema v9.13.28.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class StoresClient : IStoreService
    {
        private readonly HttpClient _httpClient;
        private readonly Lazy<JsonSerializerSettings> _settings;

        public StoresClient(string baseUrl, HttpClient httpClient)
        {
            BaseUrl = baseUrl;
            _httpClient = httpClient;
            _settings = new Lazy<JsonSerializerSettings>(() =>
            {
                var settings = new JsonSerializerSettings();
                UpdateJsonSerializerSettings(settings);
                return settings;
            });
        }

        public string BaseUrl { get; set; } = "";

        public JsonSerializerSettings JsonSerializerSettings => _settings.Value;

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of
        ///     cancellation.
        /// </param>
        public async Task<IDictionary<string, string>> AllAsync(CancellationToken cancellationToken = default)
        {
            var urlBuilder_ = new StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/api/store");

            var client_ = _httpClient;
            using (var request_ = new HttpRequestMessage())
            {
                request_.Method = new HttpMethod("GET");
                request_.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                PrepareRequest(client_, request_, urlBuilder_);
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new Uri(url_, UriKind.RelativeOrAbsolute);
                PrepareRequest(client_, request_, url_);

                var response_ = await client_
                    .SendAsync(request_, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);
                try
                {
                    var headers_ = response_.Headers.ToDictionary(h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                        {
                            headers_[item_.Key] = item_.Value;
                        }
                    }

                    ProcessResponse(client_, response_);

                    var status_ = ((int) response_.StatusCode).ToString();
                    if (status_ == "200")
                    {
                        var responseData_ = response_.Content == null
                            ? null
                            : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var result_ = default(IDictionary<string, string>);
                        try
                        {
                            result_ = JsonConvert.DeserializeObject<IDictionary<string, string>>(responseData_,
                                _settings.Value);
                            return result_;
                        }
                        catch (Exception exception_)
                        {
                            throw new SwaggerException("Could not deserialize the response body.",
                                (int) response_.StatusCode, responseData_, headers_, exception_);
                        }
                    }
                    else if (status_ != "200" && status_ != "204")
                    {
                        var responseData_ = response_.Content == null
                            ? null
                            : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new SwaggerException(
                            "The HTTP status code of the response was not expected (" + (int) response_.StatusCode +
                            ").", (int) response_.StatusCode, responseData_, headers_, null);
                    }

                    return default;
                }
                finally
                {
                    if (response_ != null)
                    {
                        response_.Dispose();
                    }
                }
            }
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of
        ///     cancellation.
        /// </param>
        public async Task<string> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            var urlBuilder_ = new StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/api/store/{id}");
            urlBuilder_.Replace("{id}", Uri.EscapeDataString(ConvertToString(id, CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            using (var request_ = new HttpRequestMessage())
            {
                request_.Method = new HttpMethod("GET");
                request_.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                PrepareRequest(client_, request_, urlBuilder_);
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new Uri(url_, UriKind.RelativeOrAbsolute);
                PrepareRequest(client_, request_, url_);

                var response_ = await client_
                    .SendAsync(request_, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);
                try
                {
                    var headers_ = response_.Headers.ToDictionary(h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                        {
                            headers_[item_.Key] = item_.Value;
                        }
                    }

                    ProcessResponse(client_, response_);

                    var status_ = ((int) response_.StatusCode).ToString();
                    if (status_ == "200")
                    {
                        var responseData_ = response_.Content == null
                            ? null
                            : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var result_ = default(string);
                        try
                        {
                            result_ = JsonConvert.DeserializeObject<string>(responseData_, _settings.Value);
                            return result_;
                        }
                        catch (Exception exception_)
                        {
                            throw new SwaggerException("Could not deserialize the response body.",
                                (int) response_.StatusCode, responseData_, headers_, exception_);
                        }
                    }
                    else if (status_ != "200" && status_ != "204")
                    {
                        var responseData_ = response_.Content == null
                            ? null
                            : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new SwaggerException(
                            "The HTTP status code of the response was not expected (" + (int) response_.StatusCode +
                            ").", (int) response_.StatusCode, responseData_, headers_, null);
                    }

                    return default;
                }
                finally
                {
                    if (response_ != null)
                    {
                        response_.Dispose();
                    }
                }
            }
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of
        ///     cancellation.
        /// </param>
        public async Task<string> SetAsync(string id, string value, CancellationToken cancellationToken = default)
        {
            var urlBuilder_ = new StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/api/store/{id}");
            urlBuilder_.Replace("{id}", Uri.EscapeDataString(ConvertToString(id, CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            using (var request_ = new HttpRequestMessage())
            {
                var content_ = new StringContent(JsonConvert.SerializeObject(value, _settings.Value));
                content_.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new HttpMethod("PUT");
                request_.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                PrepareRequest(client_, request_, urlBuilder_);
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new Uri(url_, UriKind.RelativeOrAbsolute);
                PrepareRequest(client_, request_, url_);

                var response_ = await client_
                    .SendAsync(request_, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);
                try
                {
                    var headers_ = response_.Headers.ToDictionary(h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                        {
                            headers_[item_.Key] = item_.Value;
                        }
                    }

                    ProcessResponse(client_, response_);

                    var status_ = ((int) response_.StatusCode).ToString();
                    if (status_ == "200")
                    {
                        var responseData_ = response_.Content == null
                            ? null
                            : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var result_ = default(string);
                        try
                        {
                            result_ = JsonConvert.DeserializeObject<string>(responseData_, _settings.Value);
                            return result_;
                        }
                        catch (Exception exception_)
                        {
                            throw new SwaggerException("Could not deserialize the response body.",
                                (int) response_.StatusCode, responseData_, headers_, exception_);
                        }
                    }
                    else if (status_ != "200" && status_ != "204")
                    {
                        var responseData_ = response_.Content == null
                            ? null
                            : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new SwaggerException(
                            "The HTTP status code of the response was not expected (" + (int) response_.StatusCode +
                            ").", (int) response_.StatusCode, responseData_, headers_, null);
                    }

                    return default;
                }
                finally
                {
                    if (response_ != null)
                    {
                        response_.Dispose();
                    }
                }
            }
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of
        ///     cancellation.
        /// </param>
        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var urlBuilder_ = new StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/api/store/{id}");
            urlBuilder_.Replace("{id}", Uri.EscapeDataString(ConvertToString(id, CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            using (var request_ = new HttpRequestMessage())
            {
                request_.Method = new HttpMethod("DELETE");

                PrepareRequest(client_, request_, urlBuilder_);
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new Uri(url_, UriKind.RelativeOrAbsolute);
                PrepareRequest(client_, request_, url_);

                var response_ = await client_
                    .SendAsync(request_, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);
                try
                {
                    var headers_ = response_.Headers.ToDictionary(h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                        {
                            headers_[item_.Key] = item_.Value;
                        }
                    }

                    ProcessResponse(client_, response_);

                    var status_ = ((int) response_.StatusCode).ToString();
                    if (status_ == "200")
                    {
                    }
                    else if (status_ != "200" && status_ != "204")
                    {
                        var responseData_ = response_.Content == null
                            ? null
                            : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new SwaggerException(
                            "The HTTP status code of the response was not expected (" + (int) response_.StatusCode +
                            ").", (int) response_.StatusCode, responseData_, headers_, null);
                    }
                }
                finally
                {
                    if (response_ != null)
                    {
                        response_.Dispose();
                    }
                }
            }
        }

        partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings);
        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url);
        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder);
        partial void ProcessResponse(HttpClient client, HttpResponseMessage response);

        private string ConvertToString(object value, CultureInfo cultureInfo)
        {
            if (value is Enum)
            {
                var name = Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = value.GetType().GetTypeInfo().GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = field.GetCustomAttribute(typeof(EnumMemberAttribute))
                            as EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value;
                        }
                    }
                }
            }
            else if (value is bool)
            {
                return Convert.ToString(value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return Convert.ToBase64String((byte[]) value);
            }
            else if (value != null && value.GetType().IsArray)
            {
                var array = ((Array) value).OfType<object>();
                return string.Join(",", array.Select(o => ConvertToString(o, cultureInfo)));
            }

            return Convert.ToString(value, cultureInfo);
        }
    }
#pragma warning restore
}