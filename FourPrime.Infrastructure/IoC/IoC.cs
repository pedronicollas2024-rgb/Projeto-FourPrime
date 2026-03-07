using FourPrime.Application.Interfaces;
using FourPrime.Infrastructure.Connection;
using FourPrime.Infrastructure.Repositories;
using FourPrime.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace FourPrime.Infrastructure.IoC
{
    public static class IoC
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddFourPrimeDb(connectionString);

            // Repositórios (implementam as interfaces da Application)
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ISessaoRepository, SessaoRepository>();
            services.AddScoped<ITokenRecuperacaoSenhaRepository, TokenRecuperacaoSenhaRepository>();

            // Serviços técnicos
            services.AddScoped<ICriptografiaSenha, CriptografiaSenha>();

            return services;
        }
    }
}
