using Xunit;

namespace Nethium.Consul.Test
{
    public class ConsulTest
    {
        [Fact]
        public void ConsulBuilderCanBuildClient()
        {
            var consulBuilder = new ConsulBuilder(new ConsulHandler { ClientConfigurationOptions = c => {} });
            var consulClient = consulBuilder.Build();
            Assert.NotNull(consulClient);
        }
    }
}