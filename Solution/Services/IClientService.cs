using System.Net.Http;
using System.Threading.Tasks;

namespace Pets.Services
{
    public interface IClientService
    {
        Task<HttpResponseMessage> Get(string url);
    }
}
