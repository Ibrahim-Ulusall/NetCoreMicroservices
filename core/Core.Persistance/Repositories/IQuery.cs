namespace Core.Persistance.Repositories.Interfaces;

public interface IQuery<TEntity>
{
    IQueryable<TEntity> Query();
}
