using Microsoft.IdentityModel.Tokens;
using OAuth20.Server.Common;
using OAuth20.Server.Enumeration;
using OAuth20.Server.Models;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.OauthResponse;
using OAuth20.Server.Services.CodeService;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OAuth20.Server.Services.Grants
{
    public class AuthorizationCodeGrantHandler : GrantHandlerBase
    {
        private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(10);

        private readonly ICodeStoreService _codeStoreService;

        public AuthorizationCodeGrantHandler(IClientService clientService, ITokenIssuer tokenIssuer, ICodeStoreService codeStoreService)
            : base(clientService, tokenIssuer)
        {
            _codeStoreService = codeStoreService;
        }

        public override string GrantType => AuthorizationGrantTypesEnum.AuthorizationCode.GetEnumDescription();

        // clients in the store are registered with the "code" grant
        protected override bool ClientAllowsGrant(Client client) =>
            client.GrantTypes?.Contains(AuthorizationGrantTypesEnum.Code.GetEnumDescription()) == true
            || base.ClientAllowsGrant(client);

        protected override async Task<TokenResponse> HandleAsync(TokenRequest request, Client client)
        {
            var invalidGrant = ErrorTypeEnum.InvalidGrant.GetEnumDescription();

            var code = await _codeStoreService.GetClientDataByCodeAsync(request.code);
            if (code == null)
                return Error(invalidGrant, "authorization code is invalid");

            // a code is single use, remove it no matter what the outcome is
            await _codeStoreService.RemoveClientDataByCodeAsync(request.code);

            if (code.ClientId != request.client_id)
                return Error(invalidGrant, "authorization code was issued to another client");

            if (DateTime.UtcNow - code.CreationTime > CodeLifetime)
                return Error(invalidGrant, "authorization code is expired");

            if (!string.IsNullOrEmpty(request.redirect_uri) && request.redirect_uri != code.RedirectUri)
                return Error(invalidGrant, "redirect_uri does not match the authorization request");

            if (client.UsePkce && !CodeVerifierMatches(request.code_verifier, code.CodeChallenge, code.CodeChallengeMethod))
                return Error(invalidGrant, "code_verifier is invalid");

            string idToken = string.Empty;
            string userId = null;
            if (code.IsOpenId)
            {
                if (code.Subject?.Identity?.IsAuthenticated != true)
                    return Error(invalidGrant, "user is not authenticated");

                var userName = code.Subject.Identity.Name;
                userId = code.Subject.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
                    return Error(invalidGrant, "user identity is incomplete");

                idToken = TokenIssuer.CreateIdToken(client, userId, userName, code.Nonce);
            }

            var scopes = code.RequestedScopes.Where(x => !OAuth2ServerHelpers.OpenIdConnectScopes.Contains(x));
            var accessToken = TokenIssuer.CreateAccessToken(scopes, client, userId);

            return new TokenResponse { access_token = accessToken.AccessToken, id_token = idToken, code = request.code };
        }

        private static bool CodeVerifierMatches(string verifier, string challenge, string method)
        {
            if (string.IsNullOrEmpty(verifier))
                return false;

            if (method == Constants.ChallengeMethod.Plain)
                return verifier == challenge;

            if (method == Constants.ChallengeMethod.SHA256)
            {
                using var sha = SHA256.Create();
                var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(verifier));
                return Base64UrlEncoder.Encode(hash) == challenge;
            }

            return false;
        }
    }
}
