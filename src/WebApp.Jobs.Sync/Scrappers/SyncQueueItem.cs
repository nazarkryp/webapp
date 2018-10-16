using WebApp.Domain.Entities;
using WebApp.Studios;

namespace WebApp.Jobs.Sync.Scrappers
{
    public class SyncQueueItem
    {
        public SyncDetails SyncDetails { get; set; }

        public bool Completed { get; set; }

        public IStudioClient StudioClient { get; set; }
    }
}
