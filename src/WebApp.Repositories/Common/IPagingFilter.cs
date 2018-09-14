namespace WebApp.Repositories.Common
{
    public interface IPagingFilter
    {
        string[] OrderBy { get; set; }

        int? Page { get; set; }

        int? Size { get; set; }
    }
}
