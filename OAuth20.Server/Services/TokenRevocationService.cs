using Microsoft.AspNetCore.Http;
using OAuth20.Server.Models.Context;
using OAuth20.Server.OauthResponse;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OAuth20.Server.Common;
using System.Reflection;

namespace OAuth20.Server.Services
{
    public class TokenRevocationService : ITokenRevocationService
    {
        private readonly BaseDBContext _dbContext;
        public TokenRevocationService(BaseDBContext context)
        {
            _dbContext = context;
        }

        public async Task<TokenRecovationResponse> RevokeTokenAsync(HttpContext httpContext, string clientId)
        {
            var response = new TokenRecovationResponse() { Succeeded = true };
            if (httpContext.Request.ContentType != Constants.ContentTypeSupported.XwwwFormUrlEncoded)
            {
                response.Succeeded = false;
                response.Error = "not supported content type";
            }
            string token = httpContext.Request.Form["token"];
            string tokenTypeHint = httpContext.Request.Form["token_type_hint"];

            var oauthToken = await _dbContext.OAuthTokens
                .Where(x => x.Token == token && x.ClientId == clientId &&
                (string.IsNullOrWhiteSpace(tokenTypeHint) || tokenTypeHint == x.TokenTypeHint))
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (oauthToken != null)
            {
                var res = _dbContext.OAuthTokens.Update(oauthToken);
                await _dbContext.SaveChangesAsync();
            }
            return response;
        }
    }
}
