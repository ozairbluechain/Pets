using Microsoft.Extensions.Logging;
using Moq;
using Pets.Models;
using Pets.Services;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Pets.Test
{
    public class PetServiceTests
    {
        [Fact]
        public async Task GetOwnersAndPets_WithValidHttpResponse_ShouldReturnAllPetsandOwners()
        {
            var url = "http://www.dummyurl.com";
            var json = "[{\"name\":\"Bob\",\"gender\":\"Male\",\"age\":23,\"pets\":[{\"name\":\"Garfield\",\"type\":\"Cat\"},{\"name\":\"Fido\",\"type\":\"Dog\"}]}]";
            var petService = TestHelper.CreateMockedServices(HttpStatusCode.OK, json);
            
            var owners = await petService.GetOwnersAndPets(url);

            Assert.Single(owners);
            Assert.Equal(2, owners.First().Pets.Count());

            Assert.Equal(23, owners.First().Age);
            Assert.Equal(Models.Gender.Male, owners.First().Gender);
            Assert.Equal("Bob", owners.First().Name);
            Assert.Equal("Garfield", owners.First().Pets.First().Name);
            Assert.Equal(PetType.Cat, owners.First().Pets.First().Type);
            Assert.Equal("Fido", owners.First().Pets.Last().Name);
            Assert.Equal(PetType.Dog, owners.First().Pets.Last().Type);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.GatewayTimeout)]
        public async Task GetOwnersAndPets_WithInvalidHttpResponse_ShouldThrowException(HttpStatusCode httpStatusCode)
        {
            var url = "http://www.dummyurl.com";
            var json = "[{\"name\":\"Bob\",\"gender\":\"Male\",\"age\":23,\"pets\":[{\"name\":\"Garfield\",\"type\":\"Cat\"},{\"name\":\"Fido\",\"type\":\"Dog\"}]}]";

            var petService = TestHelper.CreateMockedServices(httpStatusCode, json);
            
            await Assert.ThrowsAsync<Exception>(async () => await petService.GetOwnersAndPets(url));
        }

        [Fact]
        public async Task GetOwnersAndPets_WithInvalidJsonResponse_ShouldThrowException()
        {
            var url = "http://www.dummyurl.com";
            var json = "some invalid response";

            var petService = TestHelper.CreateMockedServices(HttpStatusCode.OK, json);

            await Assert.ThrowsAsync<Exception>(async () => await petService.GetOwnersAndPets(url));
        }
    }
}
