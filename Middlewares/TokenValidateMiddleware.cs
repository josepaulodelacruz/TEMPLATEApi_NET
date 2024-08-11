using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TemplateAPI.Services;

namespace TemplateAPI.Middlewares
{
    public class TokenValidateMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuthService _service;

        public TokenValidateMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _service = new AuthService(conString: configuration.GetConnectionString("database"));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Retrieve the token from the Authorization header
            string token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            Debug.WriteLine(token);

            var response = await _service.ValidateToken(token);

            if (!response.IsError)
            {
                await _next(context);
            } else
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
            }
        }
    }

    public class TokenValidationPipeline
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<TokenValidateMiddleware>();
        }
    }
}
