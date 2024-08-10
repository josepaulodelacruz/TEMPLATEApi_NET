using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TemplateAPI.Middlewares
{
    public class TokenValidateMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidateMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Debug.WriteLine("validating authenticated key");

            bool authValidated = true;

            if (authValidated)
            {
                Debug.WriteLine("validating authenticated key success");
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
