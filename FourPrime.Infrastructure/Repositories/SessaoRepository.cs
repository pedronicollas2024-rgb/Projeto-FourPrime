using FourPrime.Application.Interfaces;
using FourPrime.Domain.Entities;
using FourPrime.Infrastructure.Database;
using FourPrime.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FourPrime.Infrastructure.Repositories
{
    public class SessaoRepository : ISessaoRepository
    {
        private readonly AppDbContext _db;
        public SessaoRepository(AppDbContext db) => _db = db;

        public async Task CriarAsync(Sessao sessao)
        {
            _db.Sessoes.Add(sessao);
            await _db.SaveChangesAsync();
        }

        public Task<Sessao?> ObterPorTokenAsync(string tokenSessao) =>
            _db.Sessoes.FirstOrDefaultAsync(x => x.TokenSessao == tokenSessao);

        public async Task EncerrarAsync(Guid sessaoId)
        {
            var sessao = await _db.Sessoes.FirstOrDefaultAsync(x => x.Id == sessaoId);
            if (sessao == null) return;

            sessao.Ativa = false;
            sessao.EncerradaEm = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }
    }
}
