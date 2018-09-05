namespace WebApp.Domain.Entities
{
    public class Media
    {
        public int MediaId { get; set; }

        public string ObjectId { get; set; }

        public string OriginalUri { get; set; }

        public string Thumbnail { get; set; }

        public string Small { get; set; }

        public string Description { get; set; }
    }
}
