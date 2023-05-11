using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using OAuth20.Server.Services;
using OAuth20.Server.Validations;
using System;
using System.Text;
using System.Threading.Tasks;

namespace OAuth20.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenRevocationController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenRevocationValidation _tokenRevocationValidation;
        private readonly ITokenRevocationService _tokenRevocationService;
        public TokenRevocationController(IHttpContextAccessor httpContextAccessor,
            ITokenRevocationValidation tokenRevocationValidation,
            ITokenRevocationService tokenRevocationService)
        {
            _httpContextAccessor = httpContextAccessor;
            _tokenRevocationValidation = tokenRevocationValidation;
            _tokenRevocationService = tokenRevocationService;
        }

        [HttpPost("RevokToken")]
        public async Task<IActionResult> RevokToken()
        {
            var clientValidationResult = await _tokenRevocationValidation.ValidateAsync(_httpContextAccessor.HttpContext);
            if (!clientValidationResult.Succeeded)
                return Unauthorized(clientValidationResult.Error);

            var tokenRevokeResult = await _tokenRevocationService.RevokeTokenAsync(_httpContextAccessor.HttpContext,
                clientValidationResult.ClientId);

            if (tokenRevokeResult.Succeeded)
                return Ok("token_revoked");
            return NotFound(tokenRevokeResult.Error);
        }
    }
}
