namespace WebApp.Infrastructure.Configuration
{
    public interface IConfigurationProvider
    {
        string Get(string key);
    }
}
