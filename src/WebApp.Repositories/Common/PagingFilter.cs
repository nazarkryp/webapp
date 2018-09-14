namespace WebApp.Repositories.Common
{
    public class PagingFilter : IPagingFilter
    {
        public string[] OrderBy { get; set; }

        public int? Page { get; set; }

        public int? Size { get; set; }
    }
}
