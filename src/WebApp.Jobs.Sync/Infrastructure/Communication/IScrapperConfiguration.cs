namespace WebApp.Jobs.Sync.Infrastructure.Communication
{
    public interface IScrapperConfiguration
    {
        string[] Proxies { get; }
    }
}
