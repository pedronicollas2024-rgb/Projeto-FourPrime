using FourPrime.Application.Servicos;
using FourPrime.Infrastructure.Database;
using FourPrime.Infrastructure.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace FourPrime.Ul
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();


            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var services = new ServiceCollection();

            // Infra: DbContext + repos + criptografia
            services.AddInfrastructure(connectionString!);

            // Application services
            services.AddScoped<AutenticacaoService>();

            // Forms
            services.AddTransient<FrmLoginNovo>();

            var provider = services.BuildServiceProvider();

            // migrations + seed
            DatabaseInitializer.Inicializar(provider);

            // abre login via DI
            var formLogin = provider.GetRequiredService<FrmLoginNovo>();
            System.Windows.Forms.Application.Run(formLogin);
        }
    }
}