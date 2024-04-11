using CatalogService.Api.Domain;
using Core.Persistance.Repositories.Interfaces;

namespace CatalogService.Api.Application.Services;
public interface ICatalogBrandRepository : IAsyncRepository<CatalogBrand, int>, IRepository<CatalogBrand, int>
{
}
