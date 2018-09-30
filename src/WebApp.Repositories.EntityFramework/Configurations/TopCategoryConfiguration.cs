using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Configurations
{
    internal class TopCategoryConfiguration : IEntityTypeConfiguration<TopCategory>
    {
        private const string TableName = "TopCategories";

        public void Configure(EntityTypeBuilder<TopCategory> builder)
        {
            builder.ToTable(TableName);
            builder.HasKey(e => e.TopCategoryId);

            builder.HasOne(e => e.Category);
        }
    }
}
