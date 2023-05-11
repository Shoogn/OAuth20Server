using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using OAuth20.Server.OauthResponse;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OAuth20.Server.Services
{
    public class TokenIntrospectionService : ITokenIntrospectionService
    {
        public Task<TokenIntrospectionResponse> IntospectTokenAsync(HttpContext httpContext)
        {

            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();

            string publicPrivateKey = File.ReadAllText("PublicPrivateKey.xml");
            provider.FromXmlString(publicPrivateKey);

            RsaSecurityKey rsaSecurityKey = new RsaSecurityKey(provider);

            throw new System.NotImplementedException();
        }
    }
}
