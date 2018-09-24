using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using WebApp.Web.Models;

namespace WebApp.Web.Infrastructure.Filters
{
    public class UnauthorizedFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var error = new ErrorResult(HttpStatusCode.Unauthorized, "UNAUTHORIZED 401");

            var result = new JsonResult(error)
            {
                StatusCode = (int)HttpStatusCode.Unauthorized
            };

            context.Result = result;
        }
    }
}
