using Microsoft.EntityFrameworkCore;

using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Context
{
    internal class WebAppDbContext : DbContext, IWebAppDbContext
    {
        public WebAppDbContext(DbContextOptions<WebAppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Media> Media { get; set; }
    }
}
