using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Configurations
{
    internal class StudioConfiguration : IEntityTypeConfiguration<Studio>
    {
        public void Configure(EntityTypeBuilder<Studio> builder)
        {
            builder.HasKey(e => e.StudioId);

            builder.HasMany(e => e.Movies)
                .WithOne(e => e.Studio)
                .HasForeignKey(e => e.StudioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.SyncDetails)
                .WithOne(e => e.Studio)
                .HasForeignKey<SyncDetails>(e => e.StudioId);
        }
    }
}
