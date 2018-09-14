using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Configurations
{
    internal class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        private const string TableName = "Attachments";

        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(e => e.AttachmentId);
        }
    }
}
