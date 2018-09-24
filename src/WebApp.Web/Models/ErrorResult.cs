using System.Collections.Generic;
using System.Net;

namespace WebApp.Web.Models
{
    public class ErrorResult
    {
        public ErrorResult(HttpStatusCode statusCode, string message)
            : this(statusCode, statusCode.ToString(), message)
        {
        }

        public ErrorResult(HttpStatusCode statusCode, string type, string message)
        {
            Error = new ErrorDetails
            {
                Status = (int)statusCode,
                Type = type,
                Message = message
            };
        }

        public ErrorResult(HttpStatusCode statusCode, IEnumerable<string> modelState)
        {
            Error = new ErrorDetails()
            {
                Status = (int)statusCode,
                Type = statusCode.ToString(),
                ModelState = modelState
            };
        }

        public ErrorDetails Error { get; }
    }
}
