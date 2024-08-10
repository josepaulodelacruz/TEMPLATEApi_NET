using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TemplateAPI.Models;
using TemplateAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TemplateAPI.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/auth/register
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(user, new ValidationContext(user), validationResults);
            var response = new Response();

            //encrypt password
            user.Password = user.EncryptPassword(user.Password);

            response = await _authService.Register(user);

            if(!response.IsError)
            {
                return new BadRequestObjectResult(response);
            }

            return Ok(response);
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(user, new ValidationContext(user), validationResults);
            var response = new Response();

            //encrypt password
            user.Password = user.EncryptPassword(user.Password);

            response = await _authService.Login(user);

            if(response.IsError)
            {
                return new BadRequestObjectResult(response);
            }

            return Ok(response);


        }

    }
}
