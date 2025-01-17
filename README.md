### OAuth20Server
This is OAtuh 2.0 Server and OpenId Connect Provider, this OAuth server is not a complete one,
but by using it your users can login and register, also they can obtain access token and Idp token too.
If you want to create this project from scratch please read my step by step explained article from [here](https://dev.to/mohammedahmed/build-your-own-oauth-20-server-and-openid-connect-provider-in-aspnet-core-60-1g1m) 

---
### Get Started
This OAuth server target the latest .NET version (.NET 9) or any older version, so from here clone the project or download it. After that open the project with your prefere IDE (ex: Visual Studio or Visual Studio Code).
In the solution there is a folder named **Models**, inside this folder there is a class named **ClientStore.cs** this class accept a list of Clients and Clients here
means your applications, or the applications that you would like to register to intracte with OAuth2.0 server.

Here is the signature of the **Client** object ( you can find it in the Models folder )
```C#
    public class Client
    {
        public Client(){ }

    public string ClientName { get; set; }
    public string ClientId { get; set; }

    /// <summary>
    /// Client Password
    /// </summary>
    public string ClientSecret { get; set; }

    public IList<string> GrantTypes { get; set; }

    /// <summary>
    /// by default false
    /// </summary>
    public bool IsActive { get; set; } = false;
    public IList<string> AllowedScopes { get; set; }

    public string ClientUri { get; set; }
    public IList<string> RedirectUris { get; set; }

    public bool UsePkce { get; set; }

    /// <summary>
    /// Get or set the name of the clients/protected resource that are releated to this Client.
    /// </summary>
    public IList<string> AllowedProtectedResources { get; set; }
    }
 ```
 ### How to register your Applications?
 As I said in the prevoius step, there is a class named **ClientStore** and this object has an property named **Clients** which is IEnumerable of **Client** type
 For example, to register one client you should do that like the following:
 ```C#
  public class ClientStore
    {
        public IEnumerable<Client> Clients = new[]
        {
            new Client
            {
                ClientName = "blazorWasm",
                ClientId = "1",
                ClientSecret = "123456789",
                AllowedScopes = new[]{ "openid", "profile", "blazorWasmapi.readandwrite" },
                GrantType = GrantTypes.Code,
                IsActive = true,
                ClientUri = "https://localhost:7026",
                RedirectUri = "https://localhost:7026/signin-oidc",
                UsePkce = true,
            }
        };
    }
 ```
 You can add more clients as you need, the one that shown here, is a front-end (ASP.NET Core Application) client and that is very clear from the **AllowedScopes** property by allowing openid scope which indicate that the **OAuth20Server** will return **Idp token** to the registerd application alond side **access token**.
