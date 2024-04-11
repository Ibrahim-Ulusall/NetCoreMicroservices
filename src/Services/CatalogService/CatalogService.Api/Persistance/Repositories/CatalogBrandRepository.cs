using CatalogService.Api.Application.Services;
using CatalogService.Api.Domain;
using CatalogService.Api.Persistance.Contexts;
using Core.Persistance.Repositories;

namespace CatalogService.Api.Persistance.Repositories;

public class CatalogBrandRepository : EntityFrameworkRepository<CatalogBrand, CatalogServiceApiContext, int>, ICatalogBrandRepository
{
    public CatalogBrandRepository(CatalogServiceApiContext context) : base(context)
    {
    }
}
