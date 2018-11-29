using System.Net.Http;
using System.Text;
using Nethium.Swagger;
using Newtonsoft.Json;

#pragma warning disable CS8611 // Nullability of reference types in type of parameter doesn't match partial method declaration.
namespace Nethium.Demo.Stub
{
    public partial class AggregateClient : IStub
    {
        private ISwaggerStubHandler _handler;

        public void UseHandler(ISwaggerStubHandler handler)
        {
            _handler = handler;
        }

        partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
            _handler.UpdateJsonSerializerSettings(settings);
        }

        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
        {
            _handler.PrepareRequest(client, request, url);
        }

        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder)
        {
            _handler.PrepareRequest(client, request, urlBuilder);
        }

        partial void ProcessResponse(HttpClient client, HttpResponseMessage response)
        {
            _handler.ProcessResponse(client, response);
        }
    }
}
#pragma warning restore CS8611 // Nullability of reference types in type of parameter doesn't match partial method declaration.