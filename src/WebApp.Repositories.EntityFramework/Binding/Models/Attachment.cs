namespace WebApp.Repositories.EntityFramework.Binding.Models
{
    public class Attachment
    {
        public int AttachmentId { get; set; }

        public string Uri { get; set; }

        public Movie Movie { get; set; }
    }
}
