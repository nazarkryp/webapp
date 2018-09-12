using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Configurations
{
    internal class SyncDetailsConfiguration : IEntityTypeConfiguration<SyncDetails>
    {
        public void Configure(EntityTypeBuilder<SyncDetails> builder)
        {
            builder.ToTable("SyncDetails");

            builder.HasKey(e => e.SyncDetailsId);
        }
    }
}
