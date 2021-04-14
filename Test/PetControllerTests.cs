using Microsoft.Extensions.Logging;
using Moq;
using Pets.Models;
using Pets.Services;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Pets.Test
{
    public class PetControllerTests
    {
        [Fact]
        public async Task GetOwnersAndPets_WithCatTypeFilter_ShouldListAllCatsWithOwnerGenderInAlphabeticalOrder()
        {
            var json = "[{\"name\":\"Bob\",\"gender\":\"Male\",\"age\":23,\"pets\":[{\"name\":\"Garfield\",\"type\":\"Cat\"},{\"name\":\"Fido\",\"type\":\"Dog\"}]},{\"name\":\"Jennifer\",\"gender\":\"Female\",\"age\":18,\"pets\":[{\"name\":\"Garfield\",\"type\":\"Cat\"}]},{\"name\":\"Steve\",\"gender\":\"Male\",\"age\":45,\"pets\":null},{\"name\":\"Fred\",\"gender\":\"Male\",\"age\":40,\"pets\":[{\"name\":\"Tom\",\"type\":\"Cat\"},{\"name\":\"Max\",\"type\":\"Cat\"},{\"name\":\"Sam\",\"type\":\"Dog\"},{\"name\":\"Jim\",\"type\":\"Cat\"}]},{\"name\":\"Samantha\",\"gender\":\"Female\",\"age\":40,\"pets\":[{\"name\":\"Tabby\",\"type\":\"Cat\"}]},{\"name\":\"Alice\",\"gender\":\"Female\",\"age\":64,\"pets\":[{\"name\":\"Simba\",\"type\":\"Cat\"},{\"name\":\"Nemo\",\"type\":\"Fish\"}]}]";
            var url = "http://www.dummyurl.com";
            var petService = TestHelper.CreateMockedServices(HttpStatusCode.OK, json);
            var petController = new PetController(petService, new LoggerFactory().CreateLogger<PetController>());

            var formattedOutput = await petController.GetOwnersAndPets(url, pet => pet.Type == PetType.Cat);

            Assert.Equal("Male" +
                "\r\n\u2022 Garfield" +
                "\r\n\u2022 Jim" +
                "\r\n\u2022 Max" +
                "\r\n\u2022 Tom" +
                "\r\n\r\n" +
                "Female" +
                "\r\n\u2022 Garfield" +
                "\r\n\u2022 Tabby" +
                "\r\n\u2022 Simba", formattedOutput);
        }

        [Theory]
        [InlineData(PetType.Cat, "Male\r\n\u2022 Paul's cat\r\n\r\nFemale")]
        [InlineData(PetType.Dog, "Male\r\n\r\nFemale\r\n\u2022 Jena's dog")]
        [InlineData(PetType.Fish, "Male\r\n\u2022 Paul's fish\r\n\r\nFemale")]
        
        public async Task GetOwnersAndPets_WithSpecifiedFilter_ShouldPrintPetNamesWithOwnerGender(PetType type, string output)
        {
            var json = "[{\"name\":\"Paul\",\"gender\":\"Male\",\"age\":1,\"pets\":[{\"name\":\"Paul's cat\",\"type\":\"Cat\"},{\"name\":\"Paul's fish\",\"type\":\"Fish\"}]},{\"name\":\"Jena\",\"gender\":\"Female\",\"age\":2,\"pets\":[{\"name\":\"Jena's dog\",\"type\":\"Dog\"}]}]";
            var url = "http://www.dummyurl.com";
            var petService = TestHelper.CreateMockedServices(HttpStatusCode.OK, json);
            var petController = new PetController(petService, new LoggerFactory().CreateLogger<PetController>());

            var formattedOutput = await petController.GetOwnersAndPets(url, pet => pet.Type == type);

            Assert.Equal(output, formattedOutput);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest, "[]", "Http status error received from host: BadRequest.")]
        [InlineData(HttpStatusCode.InternalServerError, "[]", "Http status error received from host: InternalServerError.")]
        [InlineData(HttpStatusCode.OK, "[", "Error parsing response from host.")]
        public async Task GetOwnersAndPets_WithErrors_ShouldPrintErrorMessage(HttpStatusCode code, string json, string output)
        {
            var url = "http://www.dummyurl.com";
            var petService = TestHelper.CreateMockedServices(code, json);
            var petController = new PetController(petService, new LoggerFactory().CreateLogger<PetController>());
            
            var formattedOutput = await petController.GetOwnersAndPets(url, pet => pet.Type == PetType.Cat);

            Assert.Equal(output, formattedOutput);
        }
    }
}
