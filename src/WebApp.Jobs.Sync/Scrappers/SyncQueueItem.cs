using WebApp.Domain.Entities;

namespace WebApp.Jobs.Sync.Scrappers
{
    public class SyncQueueItem
    {
        public SyncDetails SyncDetails { get; set; }

        public bool Completed { get; set; }
    }
}
