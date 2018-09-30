using System;
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

        public DbSet<Category> Categories { get; set; }

        public DbSet<TopCategory> TopCategory { get; set; }

        public DbSet<MovieCategory> MovieCategories { get; set; }

        public DbSet<MovieModel> MovieModels { get; set; }

        public DbSet<Model> Models { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AttachmentConfiguration());
            builder.ApplyConfiguration(new MediaConfiguration());
            builder.ApplyConfiguration(new StudioConfiguration());
            builder.ApplyConfiguration(new MovieConfiguration());
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new TopCategoryConfiguration());
            builder.ApplyConfiguration(new MovieModelsConfiguration());
            builder.ApplyConfiguration(new ModelConfiguration());
            builder.ApplyConfiguration(new MovieCategoryConfiguration());

            base.OnModelCreating(builder);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
