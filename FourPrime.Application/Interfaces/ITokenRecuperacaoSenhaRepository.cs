using System.Threading.Tasks;
using FourPrime.Domain.Entities;

namespace FourPrime.Application.Interfaces
{
    public interface ITokenRecuperacaoSenhaRepository
    {
        Task CriarAsync(TokenRecuperacaoSenha token);
        Task<TokenRecuperacaoSenha?> ObterPorTokenAsync(string token);
        Task AtualizarAsync(TokenRecuperacaoSenha token);
    }
}
