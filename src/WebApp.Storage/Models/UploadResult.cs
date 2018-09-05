namespace WebApp.Storage.Models
{
    public interface IUploadResult<TKey>
    {
        TKey Identifier { get; set; }

        string OriginalUri { get; set; }

        string Thumbnail { get; set; }

        string Small { get; set; }
    }
}
