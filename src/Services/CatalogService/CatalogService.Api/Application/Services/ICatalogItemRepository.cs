using CatalogService.Api.Domain;
using Core.Persistance.Repositories.Interfaces;

namespace CatalogService.Api.Application.Services;

public interface ICatalogItemRepository : IAsyncRepository<CatalogItem, int>, IRepository<CatalogItem, int>
{
}