using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Configurations
{
    internal class MediaConfiguration : IEntityTypeConfiguration<Media>
    {
        private const string TableName = "Media";

        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(e => e.MediaId);
        }
    }
}
