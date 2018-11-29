using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Moq;
using Nethium.Consul;
using Nethium.DependencyInjection;
using Xunit;

namespace Nethium.ServiceDiscovery.Test
{
    public class ServiceDiscoveryTest
    {
        [Fact]
        public void ConsulServicesLocatorCanLocate()
        {
            var serviceIdentifier = new ServiceIdentifier<ServiceDiscoveryTest>("ServiceDiscoveryTest");
            var mockConsulServiceEndpoint = new Mock<IConsulServiceEndpoint>();
            mockConsulServiceEndpoint
                .Setup(se => se.GetService("ServiceDiscoveryTest", It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(
                        new QueryResult<ServiceEntry[]>
                        {
                            StatusCode = HttpStatusCode.OK,
                            Response = new []{ new ServiceEntry{ Service = new AgentService()} }
                        }
                    ));
            var consulServicesLocator = new ConsulServicesLocator(mockConsulServiceEndpoint.Object);
            var serviceLocations = consulServicesLocator.LocateService(serviceIdentifier, CancellationToken.None).Result;
            Assert.NotEmpty(serviceLocations);
        }
    }
}
