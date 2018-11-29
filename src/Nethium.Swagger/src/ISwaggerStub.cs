using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Nethium.Swagger
{
    public interface ISwaggerStubHandler
    {
        void UpdateJsonSerializerSettings(JsonSerializerSettings settings);

        void PrepareRequest(HttpClient client, HttpRequestMessage request, string url);

        void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder);

        void ProcessResponse(HttpClient client, HttpResponseMessage response);

        ISwaggerStubHandler Address(string locationAddress);

        ISwaggerStubHandler Port(int? port);

        ISwaggerStubHandler Meta(IDictionary<string, string> locationMeta);

        ISwaggerStubHandler Tags(string[] locationTags);
    }
}