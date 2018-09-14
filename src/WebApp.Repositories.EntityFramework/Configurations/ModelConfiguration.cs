using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Configurations
{
    internal class ModelConfiguration : IEntityTypeConfiguration<Model>
    {
        private const string TableName = "Models";

        public void Configure(EntityTypeBuilder<Model> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(e => e.ModelId);
        }
    }
}
