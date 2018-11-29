using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Configuration;
using Moq;
using Nethium.Consul;
using Xunit;

namespace Nethium.Configuration.Test
{
    public class ConfigurationTest
    {
        [Fact]
        public void ConsulConfigurationSourceCanBuildProvider()
        {
            var configurationOptions = new ConfigurationOptions();
            var mockConsulHandler = new Mock<IConsulHandler>();
            var mockConsulKvEndpoint = new Mock<IConsulKvEndpoint>();
            var mockConfigurationBuilder = new Mock<IConfigurationBuilder>();
            var configurationSource = new ConsulConfigurationSource(configurationOptions, mockConsulHandler.Object, mockConsulKvEndpoint.Object);
            var provider = configurationSource.Build(mockConfigurationBuilder.Object);
            Assert.NotNull(provider);
        }

        [Fact]
        public void ConsulConfigurationProviderCanGet()
        {
            var configurationOptions = new ConfigurationOptions
            {
                Prefix = "test/"
            };
            var mockConsulHandler = new Mock<IConsulHandler>();
            var mockConsulKvEndpoint = new Mock<IConsulKvEndpoint>();
            mockConsulKvEndpoint
                .Setup(e => e.Get("test/", null))
                .Returns(Task.FromResult(new QueryResult<KVPair[]>
                {
                    StatusCode = HttpStatusCode.OK,
                    Response = new []{ new KVPair("test/k") { Value = Encoding.UTF8.GetBytes("v") } }
                }));
            var provider = new ConsulConfigurationProvider(configurationOptions, mockConsulHandler.Object, mockConsulKvEndpoint.Object);
            provider.Load();
           
            Assert.True(provider.TryGet("k", out var v));
            Assert.Equal("v", v);
        }

        [Fact]
        public void ConsulConfigurationProviderCanSet()
        {
            var configurationOptions = new ConfigurationOptions
            {
                Prefix = "test/"
            };
            var mockConsulHandler = new Mock<IConsulHandler>();
            var mockConsulKvEndpoint = new Mock<IConsulKvEndpoint>();
            mockConsulKvEndpoint
                .Setup(e => e.Get("test/", null))
                .Returns(Task.FromResult(new QueryResult<KVPair[]>
                {
                    StatusCode = HttpStatusCode.OK,
                    Response = new KVPair[] { }
                }));

            mockConsulKvEndpoint
                .Setup(e => e.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken?>()))
                .Returns(Task.FromResult(new WriteResult<bool>{ Response = true, StatusCode = HttpStatusCode.OK }));

            var provider = new ConsulConfigurationProvider(configurationOptions, mockConsulHandler.Object, mockConsulKvEndpoint.Object);
            provider.Load();

            Assert.True(provider.SetAsync("k", "v").Result);
            Assert.True(provider.TryGet("k", out var v));
            Assert.Equal("v", v);
        }
    }
}
