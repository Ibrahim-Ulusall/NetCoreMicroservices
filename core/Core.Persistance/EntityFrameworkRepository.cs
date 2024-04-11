using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Core.Persistance.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.Persistance.Repositories;
public class EntityFrameworkRepository<TEntity, TContext, TId> : IAsyncRepository<TEntity, TId>, IRepository<TEntity, TId>
    where TEntity : Entity<TId>, new()
    where TContext : DbContext
{
    private readonly TContext _context;

    public EntityFrameworkRepository(TContext context) => _context = context;
    public TEntity Add(TEntity entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
        _context.Add(entity);
        _context.SaveChanges();
        return entity;
    }
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    public ICollection<TEntity> AddRange(ICollection<TEntity> entities)
    {
        entities.ToList().ForEach(entity => entity.CreatedDate = DateTime.UtcNow);
        _context.AddRange(entities);
        _context.SaveChanges();
        return entities;
    }
    public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities)
    {
        entities.ToList().ForEach(entity => entity.CreatedDate = DateTime.UtcNow);
        await _context.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
        return entities;
    }
    public bool Any(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();
        if (predicate is not null)
            queryable = queryable.Where(predicate);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        return queryable.Any();
    }
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();
        if (predicate is not null)
            queryable = queryable.Where(predicate);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        return queryable.AnyAsync(cancellationToken);
    }
    public async Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false)
    {
        await SetEntityAsDeletedAsync(entity, permanent);
        await _context.SaveChangesAsync();
        return entity;
    }
    public async Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, bool permanent = false)
    {
        SetEntityAsDeleted(entities, permanent);
        await _context.SaveChangesAsync();
        return entities;
    }
    public TEntity? Get(Expression<Func<TEntity, bool>> predicate, bool withDeleted = false, bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query();
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        return queryable.FirstOrDefault(predicate);
    }
    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate,
        bool withDeleted = false, bool enableTracking = true,
         Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include is not null)
            queryable = include(queryable);
        return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
    }
    public ICollection<TEntity> GetList(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false,
        bool enableTracking = true, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> queryable = Query();
        if (predicate is not null)
            queryable = queryable.Where(predicate);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include is not null)
            queryable = include(queryable);
        return queryable.ToList();
    }
    public async Task<ICollection<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();
        if (predicate is not null)
            queryable = queryable.Where(predicate);
        if (include is not null)
            queryable = include(queryable);
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        var result = await queryable.ToListAsync(cancellationToken);
        return result.ToList();
    }
    public IQueryable<TEntity> Query() => _context.Set<TEntity>();
    public TEntity Update(TEntity entity)
    {
        entity.UpdatedDate = DateTime.UtcNow;
        _context.Update(entity);
        _context.SaveChanges();
        return entity;
    }
    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.UpdatedDate = DateTime.UtcNow;
        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    public ICollection<TEntity> UpdateRange(ICollection<TEntity> entities)
    {
        entities.ToList().ForEach(entity => entity.UpdatedDate = DateTime.UtcNow);
        _context.UpdateRange(entities);
        _context.SaveChanges();
        return entities;
    }
    public async Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities)
    {
        entities.ToList().ForEach(entity => entity.UpdatedDate = DateTime.Now);
        _context.UpdateRange(entities);
        await _context.SaveChangesAsync();
        return entities;
    }
    protected async Task SetEntityAsDeletedAsync(TEntity entity, bool permanent)
    {

        if (!permanent)
        {
            CheckHasEntityHaveOneToOneRelation(entity);
            await setEntityAsSoftDeletedAsync(entity);
        }
        else
        {
            _context.Remove(entity);
        }

    }
    protected async void SetEntityAsDeleted(IEnumerable<TEntity> entities, bool permanent)
    {
        foreach (TEntity entity in entities)
            await SetEntityAsDeletedAsync(entity, permanent);
    }
    protected void CheckHasEntityHaveOneToOneRelation(TEntity entity)
    {

        bool hasEntityHaveOneToOneRelation =
        _context
            .Entry(entity)
            .Metadata.GetForeignKeys()
            .All(
                x =>
                    x.DependentToPrincipal?.IsCollection == true
                    || x.PrincipalToDependent?.IsCollection == true
                    || x.DependentToPrincipal?.ForeignKey.DeclaringEntityType.ClrType == entity.GetType()
            ) == false;
        if (hasEntityHaveOneToOneRelation)
            throw new InvalidOperationException(
                "Varlık bire bir ilişkiye sahiptir. Geçici Silme, aynı yabancı anahtarla yeniden giriş oluşturmaya çalışırsanız sorunlara neden olur."
            );

    }
    private async Task setEntityAsSoftDeletedAsync(IEntityTimeStamp entity)
    {

        if (entity.DeletedDate.HasValue)
            return;
        entity.DeletedDate = DateTime.UtcNow;

        var navigations = _context
            .Entry(entity)
            .Metadata.GetNavigations()
            .Where(x => x is { IsOnDependent: false, ForeignKey.DeleteBehavior: DeleteBehavior.ClientCascade or DeleteBehavior.Cascade })
            .ToList();
        foreach (INavigation? navigation in navigations)
        {
            if (navigation.TargetEntityType.IsOwned())
                continue;
            if (navigation.PropertyInfo == null)
                continue;

            object? navValue = navigation.PropertyInfo.GetValue(entity);
            if (navigation.IsCollection)
            {
                if (navValue == null)
                {
                    IQueryable query = _context.Entry(entity).Collection(navigation.PropertyInfo.Name).Query();
                    navValue = await GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType()).ToListAsync();
                    if (navValue == null)
                        continue;
                }

                foreach (IEntityTimeStamp navValueItem in (IEnumerable)navValue)
                    await setEntityAsSoftDeletedAsync(navValueItem);
            }
            else
            {
                if (navValue == null)
                {
                    IQueryable query = _context.Entry(entity).Reference(navigation.PropertyInfo.Name).Query();
                    navValue = await GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType())
                        .FirstOrDefaultAsync();
                    if (navValue == null)
                        continue;
                }

                await setEntityAsSoftDeletedAsync((IEntityTimeStamp)navValue);
            }
        }

        _context.Update(entity);

    }
    protected IQueryable<object> GetRelationLoaderQuery(IQueryable query, Type navigationPropertyType)
    {
        Type queryProviderType = query.Provider.GetType();
        MethodInfo createQueryMethod =
            queryProviderType
                .GetMethods()
                .First(m => m is { Name: nameof(query.Provider.CreateQuery), IsGenericMethod: true })
                ?.MakeGenericMethod(navigationPropertyType)
            ?? throw new InvalidOperationException("CreateQuery<TElement> yöntemi IQueryProvider'da bulunamadı.");
        var queryProviderQuery =
            (IQueryable<object>)createQueryMethod.Invoke(query.Provider, parameters: new object[] { query.Expression })!;
        return queryProviderQuery.Where(x => !((IEntityTimeStamp)x).DeletedDate.HasValue);
    }

}