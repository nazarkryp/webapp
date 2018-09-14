namespace WebApp.Jobs.Sync.Configuration
{
    internal class SyncConfiguration : ISyncConfiguration
    {
        public int MaxDegreeOfParallelism => 5;
    }
}
