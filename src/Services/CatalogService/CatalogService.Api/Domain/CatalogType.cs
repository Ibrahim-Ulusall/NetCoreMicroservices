using Core.Domain.Entities;

namespace CatalogService.Api.Domain;

public class CatalogType : Entity<int>
{
    public string Type { get; set; } = null!;
    public ICollection<CatalogItem> CatalogItems { get; set; }
}
