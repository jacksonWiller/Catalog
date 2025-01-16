using System.Collections.Generic;
using Catalog.Domain.Entities.ProductAggregate;
using Catalog.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Mappings;

internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder
            .ConfigureBaseEntity();

        builder
            .Property(category => category.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(category => category.Description)
            .IsRequired()
            .HasMaxLength(100);

    }
}