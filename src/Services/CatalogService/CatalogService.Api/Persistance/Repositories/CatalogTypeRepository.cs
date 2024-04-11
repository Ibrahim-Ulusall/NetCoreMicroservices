using CatalogService.Api.Application.Services;
using CatalogService.Api.Domain;
using CatalogService.Api.Persistance.Contexts;
using Core.Persistance.Repositories;

namespace CatalogService.Api.Persistance.Repositories;

public class CatalogTypeRepository : EntityFrameworkRepository<CatalogType, CatalogServiceApiContext, int>, ICatalogTypeRepository
{
    public CatalogTypeRepository(CatalogServiceApiContext context) : base(context)
    {
    }
}
