using Microsoft.Extensions.Logging;
using Pets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pets.Services
{
    /// <summary>
    /// This class is responsible to filter and sort list of pets and return formatted output.
    /// </summary>
    public class PetController : IPetController
    {
        private readonly IPetService _petService;
        private readonly ILogger<PetController> _logger;

        /// <summary>
        /// Constructor injecting services.
        /// </summary>
        /// <param name="petService">Pet service that fetched information from remote host</param>
        /// <param name="loggerFactory">Logger fatory to create a logger for this class</param>
        public PetController(IPetService petService, ILogger<PetController> logger)
        {
            _petService = petService;
            _logger = logger;
        }

        /// <summary>
        /// This method generates list of pets for both male and female owners.
        /// </summary>
        /// <param name="url">The address of the remote host to fetch list of owners and their pets</param>
        /// <param name="condition">The condition to apply to pets such as return only cats</param>
        /// <returns>The formatted output</returns>
        public async Task<string> GetOwnersAndPets(string url, Func<Pet, bool> condition)
        {
            try
            {
                var owners = await _petService.GetOwnersAndPets(url);
                var ownersWithACat = owners.Where(owner => owner?.Pets != null && owner.Pets.Any(pet => condition(pet)));

                var formattedOutput = new StringBuilder();
                formattedOutput.Append(GenerateOutput(ownersWithACat, Gender.Male, condition));
                formattedOutput.Append("\r\n\r\n");
                formattedOutput.Append(GenerateOutput(ownersWithACat, Gender.Female, condition));
                
                _logger.LogInformation("Returning formatted output.");
                return formattedOutput.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception occured.", ex);
                return ex.Message;
            }
        }

        /// <summary>
        /// Helper method to generate output for a specific gender
        /// </summary>
        /// <param name="owners"></param>
        /// <param name="gender"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        private StringBuilder GenerateOutput(IEnumerable<Owner> owners, Gender gender, Func<Pet, bool> condition)
        {
            var newLine = "\r\n";
            var formattedOutput = new StringBuilder();
            formattedOutput.Append(gender);
            owners.Where(owner => owner.Gender == gender).ToList().ForEach(owner =>
            {
                owner.Pets.Where(pet => condition(pet)).OrderBy(pet => pet.Name).ToList().ForEach(pet =>
                {
                    formattedOutput.Append($"{newLine}\u2022 {pet.Name}");
                });
            });
            return formattedOutput;
        }
    }
}
