using Microsoft.EntityFrameworkCore;

using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Context
{
    internal interface IWebAppDbContext : IDbContext
    {
        DbSet<Attachment> Attachments { get; set; }

        DbSet<Media> Media { get; set; }

        DbSet<Movie> Movies { get; set; }

        DbSet<Studio> Studios { get; set; }

        DbSet<SyncDetails> SyncDetails { get; set; }
    }
}
