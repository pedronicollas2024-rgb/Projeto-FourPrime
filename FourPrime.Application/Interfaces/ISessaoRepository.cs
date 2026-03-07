using System;
using System.Threading.Tasks;
using FourPrime.Domain.Entities;

namespace FourPrime.Application.Interfaces
{
    public interface ISessaoRepository
    {
        Task CriarAsync(Sessao sessao);
        Task<Sessao?> ObterPorTokenAsync(string tokenSessao);
        Task EncerrarAsync(Guid sessaoId);
    }
}
