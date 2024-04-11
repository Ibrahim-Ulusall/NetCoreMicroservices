using CatalogService.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Api.Persistance.TypeConfiguration;

public class CatalogItemTypeConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.ToTable("CatalogItems", "CatalogService");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").IsRequired();
        builder.Property(x => x.Price).HasColumnName("price").IsRequired();
        builder.Property(x => x.PictureUri).HasColumnName("picture_uri");
        builder.Property(x => x.PictureFileName).HasColumnName("picture_file_name");
        builder.Property(x => x.CatalogBrandId).HasColumnName("catalog_brand_id").IsRequired();
        builder.Property(x => x.CatalogTypeId).HasColumnName("catalog_type_id").IsRequired();
        builder.Property(x => x.CreatedDate).HasColumnName("created_date").IsRequired();
        builder.Property(x => x.DeletedDate).HasColumnName("deleted_date");
        builder.Property(x => x.UpdatedDate).HasColumnName("updated_date");

        builder.HasOne(x => x.CatalogType).WithMany(x => x.CatalogItems).HasForeignKey(x => x.CatalogTypeId);
        builder.HasOne(x => x.CatalogBrand).WithMany(x => x.CatalogItems).HasForeignKey(x => x.CatalogBrandId);

        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
    }
}
