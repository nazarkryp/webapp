using WebApp.Storage.Models;

namespace WebApp.Storage.Cloudinary.Models
{
    public class CloudinaryUploadResult : IUploadResult<string>
    {
        public string Identifier { get; set; }

        public string OriginalUri { get; set; }

        public string Thumbnail { get; set; }

        public string Small { get; set; }
    }
}
