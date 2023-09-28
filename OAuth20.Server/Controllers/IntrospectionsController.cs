using Microsoft.AspNetCore.Mvc;
using OAuth20.Server.OauthRequest;
using OAuth20.Server.Services;
using System.Threading.Tasks;

namespace OAuth20.Server.Controllers
{
    //[Route("api/[controller]")]
   // [ApiController]
    public class IntrospectionsController : ControllerBase
    {
        private readonly ITokenIntrospectionService _tokenIntrospectionService;
        public IntrospectionsController(ITokenIntrospectionService tokenIntrospectionService)
        {
            _tokenIntrospectionService = tokenIntrospectionService;
        }

       // [HttpPost("TokenIntrospect")]
        [HttpPost]
        public async Task<IActionResult> TokenIntrospect(TokenIntrospectionRequest tokenIntrospectionRequest)
        {
            var result = await _tokenIntrospectionService.IntrospectTokenAsync(tokenIntrospectionRequest);
            return Ok(result);
        }
    }
}
