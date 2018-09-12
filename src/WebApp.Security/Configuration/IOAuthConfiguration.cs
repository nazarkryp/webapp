namespace WebApp.Security.Configuration
{
    public interface IOAuthConfiguration
    {
        string ClientId { get;  }

        string ClientSecret { get; }

        string RedirectUri { get; }
    }
}
