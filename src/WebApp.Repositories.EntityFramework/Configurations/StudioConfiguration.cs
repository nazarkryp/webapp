using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Configurations
{
    internal class StudioConfiguration : IEntityTypeConfiguration<Studio>
    {
        private const string TableName = "Studios";

        public void Configure(EntityTypeBuilder<Studio> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(e => e.StudioId);

            builder.HasMany(e => e.Movies)
                .WithOne(e => e.Studio)
                .HasForeignKey(e => e.StudioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
