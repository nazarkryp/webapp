using System;
using WebApp.Repositories.Common;

namespace WebApp.Repositories.Movies
{
    public class MoviesPagingFilter : PagingFilter
    {
        public int[] Studios { get; set; }

        public string SearchQuery { get; set; }

        public string[] Categories { get; set; }

        public int[] Models { get; set; }

        public DateTime? Date { get; set; }
    }
}
