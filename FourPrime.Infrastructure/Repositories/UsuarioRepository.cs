using FourPrime.Application.Interfaces;
using FourPrime.Domain.Entities;
using FourPrime.Infrastructure.Database;
using FourPrime.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FourPrime.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _db;
        public UsuarioRepository(AppDbContext db) => _db = db;

        public Task<Usuario?> ObterPorIdAsync(Guid id) =>
            _db.Usuarios.FirstOrDefaultAsync(x => x.Id == id);

        public Task<Usuario?> ObterPorNomeDeUsuarioAsync(string nomeDeUsuario) =>
            _db.Usuarios.FirstOrDefaultAsync(x => x.NomeDeUsuario == nomeDeUsuario);

        public Task<Usuario?> ObterPorEmailAsync(string email) =>
            _db.Usuarios.FirstOrDefaultAsync(x => x.Email == email);

        public async Task CriarAsync(Usuario usuario)
        {
            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Usuario usuario)
        {
            _db.Usuarios.Update(usuario);
            await _db.SaveChangesAsync();
        }
    }
}
