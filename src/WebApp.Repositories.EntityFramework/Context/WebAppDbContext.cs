using Microsoft.EntityFrameworkCore;

using WebApp.Repositories.EntityFramework.Binding.Models;
using WebApp.Repositories.EntityFramework.Configurations;

namespace WebApp.Repositories.EntityFramework.Context
{
    public class WebAppDbContext : DbContext, IWebAppDbContext
    {
        public WebAppDbContext(DbContextOptions<WebAppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<Media> Media { get; set; }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<Studio> Studios { get; set; }

        public DbSet<SyncDetails> SyncDetails { get; set; }

        public DbSet<MovieModel> MovieModels { get; set; }

        public DbSet<Model> Models { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new MediaConfiguration());
            modelBuilder.ApplyConfiguration(new StudioConfiguration());
            modelBuilder.ApplyConfiguration(new MovieConfiguration());
            modelBuilder.ApplyConfiguration(new MovieModelsConfiguration());
            modelBuilder.ApplyConfiguration(new ModelConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=WebApp;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
    }
}
