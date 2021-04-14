using Pets.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pets.Services
{
    public interface IPetService
    {
        Task<IEnumerable<Owner>> GetOwnersAndPets(string url);
    }
}
