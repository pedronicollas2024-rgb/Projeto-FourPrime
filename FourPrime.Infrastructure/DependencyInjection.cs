
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using FourPrime.Infrastructure.Persistence;
using FourPrime.Infrastructure.Persistence.Seed;

using FourPrime.Application.Abstractions.Repositories;
using FourPrime.Application.Services;

using FourPrime.Infrastructure.Services;

using FourPrime.Infrastructure.Repositories;

namespace FourPrime.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ===============================
        // DB CONTEXT
        // ===============================
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        // ===============================
        // REPOSITORIES
        // ===============================
        services.AddScoped<ICarroRepository, CarroRepository>();
        services.AddScoped<IMarcaRepository, MarcaRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();

        // ===============================
        // SERVICES
        // ===============================
        services.AddScoped<ICarroQueryService, CarroQueryService>();
        services.AddScoped<ICatalogLookupService, CatalogLookupService>();

        // ================================
        // SEED
        // ===============================
        services.AddScoped<DataBaseSeeder>();

        return services;
    }
}
