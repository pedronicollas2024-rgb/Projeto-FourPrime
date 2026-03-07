using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FourPrime.Infrastructure.Entities;
using FourPrime.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FourPrime.Infrastructure.Persistence.Seed;

public class DataBaseSeeder
{
    private readonly IServiceProvider _services;
    private readonly ILogger<DataBaseSeeder> _logger;

    public DataBaseSeeder(IServiceProvider services, ILogger<DataBaseSeeder> logger)
    {
        _services = services;
        _logger = logger;
    }

    // Método de instância (pode ser usado por DI)
    public async Task SeedAsync()
    {
        await SeedInternalAsync(_services);
    }

    // Método estático usado pelo Program.cs
    public static async Task SeedAsync(IServiceProvider services)
    {
        await SeedInternalAsync(services);
    }

    private static async Task SeedInternalAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;
        var logger = provider.GetRequiredService<ILogger<DataBaseSeeder>>();

        try
        {
            var context = provider.GetRequiredService<AppDbContext>();
            var config = provider.GetService<IConfiguration>();

            // Aplicar migrações
            await context.Database.MigrateAsync();

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

            // ===============================
            // ROLES
            // ===============================
            string[] roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var r = await roleManager.CreateAsync(new IdentityRole(role));

                    if (!r.Succeeded)
                    {
                        logger.LogWarning(
                            "Falha ao criar role {Role}: {Errors}",
                            role,
                            string.Join(',', r.Errors.Select(e => e.Description))
                        );
                    }
                }
            }

            // ===============================
            // TIPOS DE USUÁRIO
            // ===============================
            if (!context.TiposUsuario.Any())
            {
                context.TiposUsuario.AddRange(
                    new TipoUsuario
                    {
                        Nome = "Administrador",
                        Descricao = "Usuário com acesso total"
                    },
                    new TipoUsuario
                    {
                        Nome = "Usuario",
                        Descricao = "Usuário padrão do sistema"
                    }
                );

                await context.SaveChangesAsync();

                logger.LogInformation("Tipos de usuário criados.");
            }

            // ===============================
            // ADMIN
            // ===============================
            var adminEmail =
                config?.GetValue<string>("Seed:AdminEmail") ?? "admin@fourprime.com";

            var adminPassword =
                config?.GetValue<string>("Seed:AdminPassword") ?? "Admin@1234";

            var tipoAdmin = await context.TiposUsuario
                .FirstAsync(t => t.Nome == "Administrador");

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    NomeCompleto = "Administrador",
                    Ativo = true,
                    TipoUsuarioId = tipoAdmin.Id
                };

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");

                    logger.LogInformation(
                        "Usuário admin criado: {Email}",
                        adminEmail
                    );
                }
                else
                {
                    logger.LogWarning(
                        "Falha ao criar admin: {Errors}",
                        string.Join(',', result.Errors.Select(e => e.Description))
                    );
                }
            }
            else
            {
                logger.LogInformation("Admin já existe: {Email}", adminEmail);
            }
        }
        catch (Exception ex)
        {
            var logger2 = services.GetService<ILogger<DataBaseSeeder>>();

            logger2?.LogError(ex, "Erro durante o seed do banco.");

            throw;
        }
    }
}
