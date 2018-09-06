namespace WebApp.Security.Configuration
{
    public interface IGoogleConfiguration
    {
        string ClientId { get;  }

        string ClientSecret { get; }

        string RedirectUri { get; }
    }
}
