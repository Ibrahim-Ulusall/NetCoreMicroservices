using CatalogService.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Api.Persistance.TypeConfiguration;

public class CatalogTypeTypeConfiguration : IEntityTypeConfiguration<CatalogType>
{
    public void Configure(EntityTypeBuilder<CatalogType> builder)
    {
        builder.ToTable("CatalogTypes", "CatalogService");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.Property(x => x.Type).HasColumnName("type").IsRequired();
        builder.Property(x => x.CreatedDate).HasColumnName("created_date").IsRequired();
        builder.Property(x => x.DeletedDate).HasColumnName("deleted_date");
        builder.Property(x => x.UpdatedDate).HasColumnName("updated_date");
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
    }
}
