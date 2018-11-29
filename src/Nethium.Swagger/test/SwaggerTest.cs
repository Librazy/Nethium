using Microsoft.AspNetCore.Http;
using Moq;
using Nethium.Authentication;
using Xunit;

namespace Nethium.Swagger.Test
{
    public class SwaggerTest
    {
        [Fact]
        public void SwaggerStubHandlerCanBuild()
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockAuthHandler = new Mock<IAuthHandler>();
            var swaggerStubHandler = new SwaggerStubHandler(mockHttpContextAccessor.Object, mockAuthHandler.Object);
            Assert.NotNull(swaggerStubHandler);
        }
    }
}
