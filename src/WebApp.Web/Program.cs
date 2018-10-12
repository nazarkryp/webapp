using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace WebApp.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            var builder = WebHost.CreateDefaultBuilder(args)
                                 .UseUrls("https://localhost:44397")
                .UseStartup<Startup>();

            return builder;
        }
    }
}
