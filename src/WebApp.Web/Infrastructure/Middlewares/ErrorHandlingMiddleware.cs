using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

using WebApp.Web.Models;

namespace WebApp.Web.Infrastructure.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await next(context);

           
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception = null)
        {
            var errorResult = new ErrorResult((HttpStatusCode)context.Response.StatusCode, exception?.Message);

            var errorResultJson = JsonConvert.SerializeObject(errorResult);

            return context.Response.WriteAsync(errorResultJson);
        }
    }
}
