using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OAuth20.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntrospectionsController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public IntrospectionsController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }


        //[HttpPost("introspect")]
        //public IActionResult TokenIntrospect()
        //{

        //    return new JsonResult();
        //}
    }
}
