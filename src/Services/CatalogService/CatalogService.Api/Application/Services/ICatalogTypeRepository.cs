using CatalogService.Api.Domain;
using Core.Persistance.Repositories.Interfaces;

namespace CatalogService.Api.Application.Services;

public interface ICatalogTypeRepository : IAsyncRepository<CatalogType, int>, IRepository<CatalogType, int>
{
}
