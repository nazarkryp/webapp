using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Configurations
{
    internal class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        private const string TableName = "Movies";

        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(e => e.MovieId);

            builder.HasMany(e => e.Attachments)
                .WithOne(e => e.Movie)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
