using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using PlatformsService.SyncDataService.Http;

namespace PlatformService.SyncDataService.Http
{
    public class CommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _client;
        private IConfiguration _configuration;

        public CommandDataClient(HttpClient client,IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public async Task SendPlatformToCommand(PlatformReadDto plat)
        {
            var httpContent = new StringContent (
                JsonSerializer.Serialize(plat),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _client.PostAsync($"{_configuration["CommandService"]}", httpContent);     

            if (response.IsSuccessStatusCode)    {
            Console.WriteLine("--> Sync POST to CommandService was OK");
            }
            else {

                 Console.WriteLine("--> Sync POST to CommandService was not OK");
            }
        }
    }
}