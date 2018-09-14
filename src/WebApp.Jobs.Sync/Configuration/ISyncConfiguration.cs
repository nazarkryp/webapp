namespace WebApp.Jobs.Sync.Configuration
{
    internal interface ISyncConfiguration
    {
        int MaxDegreeOfParallelism { get; }
    }
}
