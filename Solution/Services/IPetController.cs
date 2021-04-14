using Pets.Models;
using System;
using System.Threading.Tasks;

namespace Pets.Services
{
    public interface IPetController
    {
        Task<string> GetOwnersAndPets(string url, Func<Pet, bool> condition);
    }
}
