using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace OpenIdTestApp.Controllers
{

    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(string val)
        {
            string? access_token = await getToken();
            string? res = await getDataFromApi(access_token ?? "");
            return View("Index", res);
        }

        private async Task<string?> getToken()
        {
            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                values.Add(new KeyValuePair<string, string>("scope", "jwtapitestapp.read"));

                // To add more scopes
                // values.Add(new KeyValuePair<string, string>("scopes", "jwtapitestapp.remove"));


                string userName = "2";
                string password = "123456789";
                if (string.IsNullOrWhiteSpace(userName))
                    throw new ArgumentNullException(nameof(userName));

                if (password == null)
                    password = "";
                string url = "https://localhost:7275";  // This is the OAuth20.Server URI

                Uri baseUri = new Uri(url);
                client.BaseAddress = baseUri;
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.ConnectionClose = true;


                //Encoding encoding = Encoding.UTF8;
                string credential = String.Format("{0}:{1}", userName, password);
                string parameters = Convert.ToBase64String(Encoding.UTF8.GetBytes(credential));

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", parameters);
                using var req = new HttpRequestMessage(HttpMethod.Post, "/Home/Token") { Content = new FormUrlEncodedContent(values) };
                using var res = await client.SendAsync(req);

                res.EnsureSuccessStatusCode(); // If not success it will throw HttpRequestException
                string responseBody = await res.Content.ReadAsStringAsync();


                var result = System.Text.Json.JsonSerializer.Deserialize<OAuthCallingResult>(responseBody);
                return result?.access_token;
            }
        }
        public class OAuthCallingResult
        {
            public string? access_token { get; set; }
        }


        private async Task<string?> getDataFromApi(string accessToken)
        {
            using (var client = new HttpClient())
            {
                string url = "https://localhost:7065";

                Uri baseUri = new Uri(url);
                client.BaseAddress = baseUri;
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.ConnectionClose = true;

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                using var req = new HttpRequestMessage(HttpMethod.Get, "/api/Articles/GetDummyData");
                using var res = client.Send(req);

                res.EnsureSuccessStatusCode(); // If not success it will throw HttpRequestException
                string responseBody = await res.Content.ReadAsStringAsync();
                return responseBody;
            }
        }

        // For reference only
        private async Task<bool> TokenIntrospection(string token)
        {
            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("token", token));

                // To add more scopes
                // values.Add(new KeyValuePair<string, string>("scopes", "jwtapitestapp.remove"));


                string userName = "2";
                string password = "123456789";
                if (string.IsNullOrWhiteSpace(userName))
                    throw new ArgumentNullException(nameof(userName));

                if (password == null)
                    password = "";
                string url = "https://localhost:7275";  // This is the OAuth20.Server URI

                Uri baseUri = new Uri(url);
                client.BaseAddress = baseUri;
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.ConnectionClose = true;


                //Encoding encoding = Encoding.UTF8;
                string credential = String.Format("{0}:{1}", userName, password);
                string parameters = Convert.ToBase64String(Encoding.UTF8.GetBytes(credential));

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", parameters);
                using var req = new HttpRequestMessage(HttpMethod.Post, "/Introspections/TokenIntrospect") { Content = new FormUrlEncodedContent(values) };
                using var res = await client.SendAsync(req);

               // res.EnsureSuccessStatusCode(); // If not success it will throw HttpRequestException
                string responseBody = await res.Content.ReadAsStringAsync();

                return true;
            }
        }
    }
}
