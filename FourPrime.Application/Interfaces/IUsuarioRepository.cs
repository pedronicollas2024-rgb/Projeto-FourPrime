using System;
using System.Threading.Tasks;
using FourPrime.Domain.Entities;

namespace FourPrime.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorIdAsync(Guid id);
        Task<Usuario?> ObterPorNomeDeUsuarioAsync(string nomeDeUsuario);
        Task<Usuario?> ObterPorEmailAsync(string email);

        Task CriarAsync(Usuario usuario);
        Task AtualizarAsync(Usuario usuario);
    }
}
