namespace WebApp.Storage.Cloudinary.Configuration
{
    public interface ICloudinaryConfiguration
    {
        string CloudName { get; }

        string ClientSecret { get; }

        string ClientKey { get; }

        string Directory { get; }
    }
}
