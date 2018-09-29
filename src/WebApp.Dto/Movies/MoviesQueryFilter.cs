using WebApp.Dto.Common;

namespace WebApp.Dto.Movies
{
    public class MoviesQueryFilter : QueryFilter
    {
        public int[] StudioId { get; set; }

        public string Search { get; set; }

        public string[] Categories { get; set; }
    }
}
