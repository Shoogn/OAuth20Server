using Deviceflow_ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Deviceflow_ConsoleApp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("calling token enpoint is working normally");
               // int? interval = 5000;
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                var data = dbContext.DeviceFlowClients.Take(20).ToList();
                if (data.Any())
                {
                    // call token endpoint
                    foreach (var deviceCode in data)
                        await CallDeviceflowEndpointAsync(deviceCode.DeviceCode);
                }
                // TODO: interval should read from result.
                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task CallDeviceflowEndpointAsync(string deviceCode)
        {
            HttpClient client = new HttpClient();
            string baseUrl = "https://localhost:7275";
            Console.WriteLine("Call the token endpoint to get an access token");

            var value = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("client_Id", "4"),
            new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:device_code"),
            new KeyValuePair<string, string>("device_code", deviceCode)
        };

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.ConnectionClose = true;

            using var request = new HttpRequestMessage(HttpMethod.Post, "/Home/Token") { Content = new FormUrlEncodedContent(value) };
            using var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode == false)
            {
                Console.WriteLine($"Calling token endpoint is faild with this status code: {response.StatusCode}");
            }

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var deSerilaizedData = System.Text.Json.JsonSerializer.Deserialize(stream, typeof(object));

                var deSerilaizedDataAsString = deSerilaizedData.ToString();
                using (JsonDocument document = JsonDocument.Parse(deSerilaizedDataAsString))
                {
                    try
                    {
                        JsonElement root = document.RootElement;
                        var accessTokenProperty = root.GetProperty("access_token");
                        var accessTokenValue = accessTokenProperty.GetString();
                        if(!string.IsNullOrWhiteSpace( accessTokenValue ))
                        {
                            // Now you have an access token, you can apply to whatever you want
                            // remove device code
                            using var scope = _serviceScopeFactory.CreateScope();
                            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                            var en = dbContext.DeviceFlowClients.Find(deviceCode);
                            if(en != null)
                            {
                                dbContext.Remove(en);
                                dbContext.SaveChanges();
                            }
                        }

                    }
                    catch // ArgumentNullException / KeyNotFoundException
                    {
                        return;
                    }
                }
            }

        }
    }
}
