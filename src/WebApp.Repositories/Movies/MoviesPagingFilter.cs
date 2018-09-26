using WebApp.Repositories.Common;

namespace WebApp.Repositories.Movies
{
    public class MoviesPagingFilter : PagingFilter
    {
        public int[] StudioIds { get; set; }

        public string SearchQuery { get; set; }
    }
}
