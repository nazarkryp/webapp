using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Reflection;
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

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);

                if (context.Response.StatusCode != StatusCodes.Status200OK &&
                    context.Response.StatusCode != StatusCodes.Status201Created &&
                    context.Response.StatusCode != StatusCodes.Status204NoContent)
                {
                    await HandleExceptionAsync(context, null);
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var errorResult = new ErrorResult((HttpStatusCode)context.Response.StatusCode, "Something fucked up");

            var errorResultJson = JsonConvert.SerializeObject(errorResult);

            HttpResponseMessage message = new HttpResponseMessage();
            context.Response

            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(errorResultJson);
        }
    }
}
