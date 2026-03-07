using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FourPrime.Domain.Entities;
using FourPrime.Infrastructure.Security;
using FourPrime.Infrastructure.Persistence;

namespace FourPrime.Infrastructure.Database
{
    public static class DatabaseInitializer
    {
        public static void Inicializar(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Garante banco/migrations
            db.Database.Migrate();

            // SEED: cria admin se não existir
            if (!db.Usuarios.Any(u => u.NomeDeUsuario == "admin"))
            {
                var crypto = new CriptografiaSenha();

                var admin = new Usuario
                {
                    Id = Guid.NewGuid(),
                    NomeCompleto = "Administrador",
                    NomeDeUsuario = "admin",
                    Email = "admin@fourPrime.com",
                    Telefone = "",
                    HashSenha = crypto.GerarHash("Admin@123"),
                    Ativo = true,
                    EmailVerificado = true,
                    Perfil = "Admin",
                    CriadoEm = DateTime.UtcNow,
                    TentativasLoginFalhas = 0,
                    BloqueadoAte = null
                };

                db.Usuarios.Add(admin);
                db.SaveChanges();
            }
        }
    }
}