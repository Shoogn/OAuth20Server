// See https://aka.ms/new-console-template for more information
using System.Text.Json.Serialization;

await DeviceflowEndpoint.RunAsync();
Console.ReadLine();


class DeviceflowEndpoint
{

    static readonly string baseUrl = "https://localhost:7275";
    static readonly HttpClient client = new HttpClient();

    static async Task CallDeviceflowEndpointAsync()
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

        // Store the result in the save palce to call the token endpoint in a later time repeatedly  
        Console.WriteLine("Using a browser on another device, visit:");
        Console.WriteLine(deviceAuthorizationResponse.VerificationUri);
        Console.WriteLine("And enter the code:");
        Console.WriteLine(deviceAuthorizationResponse.UserCode);
        Console.WriteLine();
    }

    public static async Task RunAsync()
    {
        await CallDeviceflowEndpointAsync();

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


