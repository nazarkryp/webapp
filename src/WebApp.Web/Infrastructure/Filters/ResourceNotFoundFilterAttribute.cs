using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using WebApp.Services.Exceptions;
using WebApp.Web.Models;

namespace WebApp.Web.Infrastructure.Filters
{
    public class ResourceNotFoundFilterAttribute : ExceptionFilterAttribute
    {
        private const HttpStatusCode StatusCode = HttpStatusCode.NotFound;

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is ResourceNotFoundException)
            {
                var errorResult = new ErrorResult(StatusCode, "Resource not found");
                var jsonResult = new JsonResult(errorResult)
                {
                    StatusCode = (int)StatusCode
                };

                context.Result = jsonResult;
            }
        }
    }
}
