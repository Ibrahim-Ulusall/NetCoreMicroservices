using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Core.Persistance.Repositories.Interfaces;
public interface IAsyncRepository<TEntity, TId> : IQuery<TEntity> where TEntity : Entity<TId>, new()
{
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null,
        bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default);
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate,
        bool withDeleted = false, bool enableTracking = true,
         Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = default);
    Task<ICollection<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null,
        bool withDeleted = false, bool enableTracking = true,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = default);

    Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities);
    Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, bool permanent = false);
    Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities);
}
