using WebApp.Dto.Common;

namespace WebApp.Dto.Movies
{
    public class MoviesQueryFilter : QueryFilter
    {
        public int[] StudioIds { get; set; }

        public string SearchQuery { get; set; }
    }
}
