using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Configurations
{
    internal class MovieModelsConfiguration : IEntityTypeConfiguration<MovieModel>
    {
        private const string TableName = "MovieModels";

        public void Configure(EntityTypeBuilder<MovieModel> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(e => new { e.MovieId, e.ModelId });

            builder.HasOne(e => e.Movie)
                .WithMany(e => e.MovieModels)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Model)
                .WithMany(e => e.MovieModel)
                .HasForeignKey(e => e.ModelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
