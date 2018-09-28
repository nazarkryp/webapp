using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Configurations
{
    internal class MovieCategoryConfiguration : IEntityTypeConfiguration<MovieCategory>
    {
        private const string TableName = "MovieCategories";

        public void Configure(EntityTypeBuilder<MovieCategory> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(e => new { e.MovieId, CategoryId = e.CategoryId });

            builder.HasOne(e => e.Movie)
                .WithMany(e => e.MovieCategories)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Category)
                .WithMany(e => e.MovieCategories)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
