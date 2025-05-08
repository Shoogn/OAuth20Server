using OAuth20.Server.OauthRequest;
using OAuth20.Server.Validations.Interfaces;
using OAuth20.Server.Validations.Response;
using System.Threading.Tasks;

namespace OAuth20.Server.Validations
{
    public class GenerateTokenRequestValidation : IGenerateTokenRequestValidation
    {
        public Task<GenerateTokenValidationResponse> ValidateAsync(TokenRequest tokenRequest)
        {
            throw new System.NotImplementedException();
        }
    }
}
