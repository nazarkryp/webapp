namespace WebApp.Domain.Entities
{
    public class Studio
    {
        public int StudioId { get; set; }

        public string Name { get; set; }

        public SyncDetails SyncDetails { get; set; }
    }
}
