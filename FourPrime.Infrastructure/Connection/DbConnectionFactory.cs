using FourPrime.Infrastructure.Database;
using FourPrime.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FourPrime.Infrastructure.Connection
{
    public static class DbConnectionFactory
    {
        public static IServiceCollection AddFourPrimeDb(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(connectionString));

            return services;
        }
    }
}
