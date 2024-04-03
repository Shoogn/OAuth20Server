// See https://aka.ms/new-console-template for more information
using Deviceflow_ConsoleApp;
using Deviceflow_ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;


IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true, true).Build();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(options => options.AddConsole())
    .ConfigureHostConfiguration(options => options.AddConfiguration(config))
    .ConfigureServices((hostBuilder, services) =>
    {
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlite(hostBuilder.Configuration.GetConnectionString("DeviceFlowClientConnection"));
        });

        services.AddScoped<DeviceflowEndpoint>();
       // services.AddHostedService<Worker>();
    }).Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Console Application Start");

var deviceflowEndpointService = host.Services.GetRequiredService<DeviceflowEndpoint>();
await deviceflowEndpointService.CallDeviceflowEndpointAsync();

host.Run();
Console.ReadLine();


public class DeviceflowEndpoint
{
    private readonly DataContext _context;
    public DeviceflowEndpoint(DataContext context)
    {
        _context = context;
    }
    static readonly string baseUrl = "https://localhost:7275";

    // TODO: Change it to IHttpClientFactory
    static readonly HttpClient client = new HttpClient();

    public async Task CallDeviceflowEndpointAsync()
    {
        Console.WriteLine("Call the device flow endpoint to get user and device codes");

        var value = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("client_Id", "4"),
            new KeyValuePair<string, string>("scope", "consoledeviceapp.read")
        };

        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.ConnectionClose = true;

        using var request = new HttpRequestMessage(HttpMethod.Post, "/DeviceAuthorization") { Content = new FormUrlEncodedContent(value) };
        using var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode == false)
        {
            Console.WriteLine($"Calling DeviceAuthorization endpoint is faild with this status code: {response.StatusCode}");
        }
        var responseBody = await response.Content.ReadAsStringAsync();
        var deviceAuthorizationResponse = System.Text.Json.JsonSerializer.Deserialize<DeviceAuthorizationResponse>(responseBody);


        if(deviceAuthorizationResponse  != null )
        {
            if(deviceAuthorizationResponse.DeviceCode != null)
            {
                var data = new DeviceFlowClientEntity
                {
                    DeviceCode = deviceAuthorizationResponse.DeviceCode
                };
                _context.Add(data);
                var x = _context.SaveChanges();
            }
        }



        // Store the result in the save palce to call the token endpoint in a later time repeatedly  
        Console.WriteLine("Using a browser on another device, visit:");
        Console.WriteLine(deviceAuthorizationResponse.VerificationUri);
        Console.WriteLine("And enter the code:");
        Console.WriteLine(deviceAuthorizationResponse.UserCode);
        Console.WriteLine();


        var listAllDeviceCodes = _context.DeviceFlowClients.ToList();
        Console.WriteLine("Device Code from data base");

        foreach (var data in listAllDeviceCodes)
            Console.WriteLine(data.DeviceCode);
    }
}


class DeviceAuthorizationResponse
{
    [JsonPropertyName("device_code")]
    public string DeviceCode { get; set; }

    [JsonPropertyName("user_code")]
    public string UserCode { get; set; }

    [JsonPropertyName("verification_uri")]
    public string VerificationUri { get; set; }

    [JsonPropertyName("verification_uri_complete")]
    public string VerificationUriComplete { get; set; }

    /// <summary>
    /// Get or set the lifetime in seconds of the <see cref="DeviceCode"/> and <see cref="UserCode"/>.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("interval")]
    public int Interval { get; set; }
}


