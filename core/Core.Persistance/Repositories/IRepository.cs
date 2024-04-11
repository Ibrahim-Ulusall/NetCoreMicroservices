using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Core.Persistance.Repositories.Interfaces;

public interface IRepository<TEntity, TId> : IQuery<TEntity> where TEntity : Entity<TId>, new()
{
    TEntity Add(TEntity entity);
    TEntity Update(TEntity entity);
    bool Any(Expression<Func<TEntity, bool>>? predicate = null,
        bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default);
    TEntity? Get(Expression<Func<TEntity, bool>> predicate,
        bool withDeleted = false, bool enableTracking = true);
    ICollection<TEntity> GetList(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool withDeleted = false, bool enableTracking = true,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);

    ICollection<TEntity> AddRange(ICollection<TEntity> entities);
    ICollection<TEntity> UpdateRange(ICollection<TEntity> entities);
}