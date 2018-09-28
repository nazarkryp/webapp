using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Configurations
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        private const string TableName = "Categories";

        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable(TableName);
            builder.HasKey(e => e.CategoryId);
        }
    }
}
