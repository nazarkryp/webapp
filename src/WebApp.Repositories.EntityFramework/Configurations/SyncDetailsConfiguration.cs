using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Configurations
{
    internal class SyncDetailsConfiguration : IEntityTypeConfiguration<SyncDetails>
    {
        private const string TableName = "SyncDetails";

        public void Configure(EntityTypeBuilder<SyncDetails> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(e => e.SyncDetailsId);

            builder.HasOne(e => e.Studio)
                .WithOne()
                .HasForeignKey<Studio>(e => e.StudioId);
        }
    }
}
