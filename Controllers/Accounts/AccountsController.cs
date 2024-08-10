using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TemplateAPI.Middlewares;

namespace TemplateAPI.Controllers.Accounts
{
    [Route("api/[controller]")]
    [ApiController]
    [MiddlewareFilter(typeof(TokenValidationPipeline))]
    public class AccountsController : ControllerBase
    {

        [HttpGet]
        public string Get()
        {
            return "HELLO WORLD";
        }
    }
}
