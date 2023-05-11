using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace OAuth20.Server.Validations
{
    public interface ITokenRevocationValidation
    {
        Task<TokenRevocationValidationResponse> ValidateAsync(HttpContext context);
    }
}
