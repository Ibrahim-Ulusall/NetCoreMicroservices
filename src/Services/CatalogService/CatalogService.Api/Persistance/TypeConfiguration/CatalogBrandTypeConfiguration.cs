using CatalogService.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Api.Persistance.TypeConfiguration;
public class CatalogBrandTypeConfiguration : IEntityTypeConfiguration<CatalogBrand>
{
    public void Configure(EntityTypeBuilder<CatalogBrand> builder)
    {
        builder.ToTable("CatalogBrands", "CatalogService");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").UseIdentityColumn().IsRequired();
        builder.Property(x => x.Brand).HasColumnName("brand").IsRequired();
        builder.Property(x => x.CreatedDate).HasColumnName("created_date").IsRequired();
        builder.Property(x => x.DeletedDate).HasColumnName("deleted_date");
        builder.Property(x => x.UpdatedDate).HasColumnName("updated_date");
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
    }
}
