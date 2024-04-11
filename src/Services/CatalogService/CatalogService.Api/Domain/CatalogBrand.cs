using Core.Domain.Entities;

namespace CatalogService.Api.Domain;
public class CatalogBrand : Entity<int>
{
    public string Brand { get; set; } = null!;
    public ICollection<CatalogItem> CatalogItems { get; set; }
}
