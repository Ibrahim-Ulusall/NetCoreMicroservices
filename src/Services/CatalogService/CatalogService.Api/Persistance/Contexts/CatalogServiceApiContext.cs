using CatalogService.Api.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CatalogService.Api.Persistance.Contexts;
public class CatalogServiceApiContext : DbContext
{
    public DbSet<CatalogBrand> CatalogBrands { get; set; }
    public DbSet<CatalogItem> CatalogItems { get; set; }
    public DbSet<CatalogType> CatalogTypes { get; set; }
    public CatalogServiceApiContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        => Database.EnsureCreated();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

}
