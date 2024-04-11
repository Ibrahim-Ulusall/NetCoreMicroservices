using CatalogService.Api.Application.Services;
using CatalogService.Api.Domain;
using CatalogService.Api.Persistance.Contexts;
using Core.Persistance.Repositories;

namespace CatalogService.Api.Persistance.Repositories;
public class CatalogItemRepository : EntityFrameworkRepository<CatalogItem, CatalogServiceApiContext, int>, ICatalogItemRepository
{
    public CatalogItemRepository(CatalogServiceApiContext context) : base(context)
    {
    }
}
