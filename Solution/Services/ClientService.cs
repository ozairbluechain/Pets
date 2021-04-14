using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Pets.Services
{
    /// <summary>
    /// This class is responsible to provide http connection to a host
    /// </summary>
    public class ClientService : IClientService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ClientService> _logger;

        /// <summary>
        /// Constructor injecting services
        /// </summary>
        /// <param name="factory">Http client factory to create a client</param>
        /// <param name="loggerFactory">Logger factory to create logger</param>
        public ClientService(IHttpClientFactory factory = null, ILogger<ClientService> logger = null)
        {
            _httpClient = factory?.CreateClient("") ?? new HttpClient();
            //_logger = loggerFactory?.CreateLogger<ClientService>() ?? new NullLogger<ClientService>();
            _logger = logger;
        }

        /// <summary>
        /// Get method that implement the HTTP GET request.
        /// </summary>
        /// <param name="url">Url to connect to</param>
        /// <returns>Returns http response message</returns>
        public async virtual Task<HttpResponseMessage> Get(string url)
        {
            HttpResponseMessage response = null;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var message = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            try
            {
                response = await _httpClient.SendAsync(message, CancellationToken.None);
            }
            catch (Exception)
            {
                throw;
            }
            _logger.LogInformation($"Returning http response [{await response.Content.ReadAsStringAsync()}]");
            return response;
        }
    }
}
