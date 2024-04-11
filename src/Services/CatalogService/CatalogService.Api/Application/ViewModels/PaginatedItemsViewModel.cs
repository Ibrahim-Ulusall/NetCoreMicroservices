using Core.Domain.Entities;

namespace CatalogService.Api.Application.ViewModels;
public class PaginatedItemsViewModel<TEntity> where TEntity : class
{
    public int PageIndex { get; private set; }
    public int PageSize { get; private set; }
    public int Count { get; private set; }
    public IEnumerable<TEntity> Data { get; set; }

    public PaginatedItemsViewModel(int pageIndex, int pageSize, int count, IEnumerable<TEntity> data)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Count = count;
        Data = data;
    }
}
