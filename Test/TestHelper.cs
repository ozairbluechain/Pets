using Microsoft.Extensions.Logging;
using Moq;
using Pets.Services;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pets.Test
{
    public static class TestHelper
    {
        public static IPetService CreateMockedServices(HttpStatusCode code, string jsonResponse)
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockClient = new Mock<HttpClient>();
            mockFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockClient.Object);
            mockClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new HttpResponseMessage(code)
                {
                    Content = new StringContent(jsonResponse)
                }));
            var loggerFactory = new LoggerFactory();
            return new PetService(new ClientService(mockFactory.Object, loggerFactory.CreateLogger<ClientService>()), loggerFactory.CreateLogger<PetService>());
        }
    }
}
