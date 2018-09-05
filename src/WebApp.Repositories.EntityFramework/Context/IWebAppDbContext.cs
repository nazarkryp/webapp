using Microsoft.EntityFrameworkCore;

using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Context
{
    internal interface IWebAppDbContext : IDbContext
    {
        DbSet<Media> Media { get; set; }
    }
}
