using CatalogService.Api.Application.Services;
using CatalogService.Api.Persistance.Contexts;
using CatalogService.Api.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Extensions;
public static class CatalogServiceApiServiceRegistration
{
    public static IServiceCollection AddCatalogServiceDependencyResolvers(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<CatalogServiceApiContext>(config=> config.UseNpgsql(configuration.GetConnectionString("DefaultConnecion")));
        services.AddScoped<ICatalogItemRepository, CatalogItemRepository>();
        services.AddScoped<ICatalogTypeRepository, CatalogTypeRepository>();
        services.AddScoped<ICatalogBrandRepository, CatalogBrandRepository>();
        return services;

    }
}
