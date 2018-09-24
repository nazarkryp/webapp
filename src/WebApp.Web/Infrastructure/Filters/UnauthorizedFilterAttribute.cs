using System;
using System.Net;
using Google.Apis.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2;
using WebApp.Web.Models;

namespace WebApp.Web.Infrastructure.Filters
{
    public class UnauthorizedFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is Exception)
            {
                var code = StatusCodes.Status401Unauthorized;

                var exception = actionExecutedContext.Exception;

                var error = new ErrorResult(HttpStatusCode.Unauthorized, "UNAUTHORIZED 401");

                var result = new UnauthorizedResult();

                actionExecutedContext.Result = result;
            }
        }
    }
}
