using OAuth20.Server.OauthRequest;
using OAuth20.Server.Validations.Response;
using System.Threading.Tasks;

namespace OAuth20.Server.Validations
{
    public interface ITokenIntrospectionValidation
    {
        Task<TokenIntrospectionValidationResponse> ValidateAsync(TokenIntrospectionRequest tokenIntrospectionReques);
    }
}
