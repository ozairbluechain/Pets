using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pets.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pets.Services
{
    /// <summary>
    /// This class is responsible to fetch the list of owners and pets from the remote host.
    /// </summary>
    public class PetService : IPetService
    {
        private readonly IClientService _client;
        private readonly ILogger<PetService> _logger;

        /// <summary>
        /// Constructor injection services
        /// </summary>
        /// <param name="client">Http service to create remote connection</param>
        /// <param name="loggerFactory">Logger factory to create logger for this class</param>
        public PetService(IClientService client, ILogger<PetService> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// This method fetched list of all owners and pets from the remote url.
        /// </summary>
        /// <param name="url">The address of the remote host</param>
        /// <returns>The complete list of owner and their pets</returns>
        public async Task<IEnumerable<Owner>>GetOwnersAndPets(string url)
        {
            var httpResponse = await _client.Get(url);
            if (!httpResponse.IsSuccessStatusCode)
                throw new System.Exception($"Http status error received from host: {httpResponse.StatusCode}.");

            _logger.LogInformation("Returning owner and pets.");

            try
            {
                return JsonConvert.DeserializeObject<IEnumerable<Owner>>(await httpResponse.Content.ReadAsStringAsync());
            }
            catch(Exception ex)
            {
                throw new Exception("Error parsing response from host.", ex);
            }
        }
    }
}
