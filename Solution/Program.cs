using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pets.Services;
using System;
using System.Threading.Tasks;

namespace Pets
{
    class Program
    {
        private static ILogger<Program> _logger;
        private static IPetController _petController;

        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            _logger = host.Services.GetService<ILoggerFactory>().CreateLogger<Program>();
            _petController = host.Services.GetService<IPetController>();

            var url = "http://agl-developer-test.azurewebsites.net/people.json";
            var formattedOutput = await _petController.GetOwnersAndPets(url, pet => pet.Type == Models.PetType.Cat);
            
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine(formattedOutput);

            Console.WriteLine("\r\nPress any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// This method setups up dependency injection
        /// </summary>
        static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                            .ConfigureServices((_, services) =>
                            {
                                services.AddSingleton<IClientService, ClientService>()
                                .AddSingleton<IPetService, PetService>()
                                .AddSingleton<IPetController, PetController>()
                                .AddHttpClient();
                            }).ConfigureLogging(logging =>
                            {
                                logging.ClearProviders();
                                logging.AddFile("Logs/mylog-{Date}.txt");
                            });
            return builder;     
        }                    
    }
}