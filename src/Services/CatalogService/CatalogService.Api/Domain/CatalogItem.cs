using Core.Domain.Entities;

namespace CatalogService.Api.Domain;

public class CatalogItem : Entity<int>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? PictureFileName { get; set; }
    public string? PictureUri { get; set; }
    public int CatalogTypeId { get; set; }
    public int CatalogBrandId { get; set; }
    public CatalogBrand CatalogBrand { get; set; } = null!;
    public CatalogType CatalogType { get; set; } = null!;
}
