using System;
using System.Collections.Generic;

namespace WebApp.Infrastructure.Exceptions
{
    public class InvalidFilterCriteriaException : Exception
    {
        public InvalidFilterCriteriaException(IEnumerable<string> filters)
        {
            Filters = filters;
        }

        public IEnumerable<string> Filters { get; set; }
    }
}
