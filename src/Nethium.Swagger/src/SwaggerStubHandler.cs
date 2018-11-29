using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Nethium.Authentication;
using Newtonsoft.Json;

namespace Nethium.Swagger
{
    public class SwaggerStubHandler : ISwaggerStubHandler
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IAuthHandler _handler;
        private string[]? _locationTags;

        public SwaggerStubHandler(IHttpContextAccessor accessor, IAuthHandler handler)
        {
            _accessor = accessor;
            _handler = handler;
        }

        public void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
        }

        public void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
        {
        }

        public void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder)
        {
            var server = (from tag in _locationTags
                          where tag.StartsWith("server-")
                          select tag.Substring("server-".Length))
                .Single();
            var requestHeader = _accessor.HttpContext?.Request.Headers["Authorization"];
            var reqJwt = requestHeader?.SingleOrDefault()?.Substring(7);
            if (reqJwt == null)
            {
                return;
            }

            var jwt = _handler.ProxyToken(reqJwt, server);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer ", jwt);
        }

        public void ProcessResponse(HttpClient client, HttpResponseMessage response)
        {
        }

        public ISwaggerStubHandler Address(string locationAddress) => this;

        public ISwaggerStubHandler Port(int? port) => this;

        public ISwaggerStubHandler Meta(IDictionary<string, string> locationMeta) => this;

        public ISwaggerStubHandler Tags(string[] locationTags)
        {
            _locationTags = locationTags;
            return this;
        }
    }
}