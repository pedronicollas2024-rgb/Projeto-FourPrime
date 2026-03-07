using FourPrime.Application.Interfaces;
using FourPrime.Domain.Entities;
using FourPrime.Infrastructure.Database;
using FourPrime.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FourPrime.Infrastructure.Repositories
{
    public class TokenRecuperacaoSenhaRepository : ITokenRecuperacaoSenhaRepository
    {
        private readonly AppDbContext _db;
        public TokenRecuperacaoSenhaRepository(AppDbContext db) => _db = db;

        public async Task CriarAsync(TokenRecuperacaoSenha token)
        {
            _db.TokensRecuperacaoSenha.Add(token);
            await _db.SaveChangesAsync();
        }

        public Task<TokenRecuperacaoSenha?> ObterPorTokenAsync(string token) =>
            _db.TokensRecuperacaoSenha.FirstOrDefaultAsync(x => x.Token == token);

        public async Task AtualizarAsync(TokenRecuperacaoSenha token)
        {
            _db.TokensRecuperacaoSenha.Update(token);
            await _db.SaveChangesAsync();
        }
    }
}
