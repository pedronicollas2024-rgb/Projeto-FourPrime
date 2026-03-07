using Microsoft.AspNetCore.Identity;
using FourPrime.Domain.Entities;

namespace FourPrime.Infrastructure.Entities;

public class ApplicationUser : IdentityUser
{
    public string? NomeCompleto { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public bool Ativo { get; set; } = true;

    // Relacionamento com TipoUsuario (opcional por enquanto)
    public int? TipoUsuarioId { get; set; }

    public virtual TipoUsuario? TipoUsuario { get; set; }
}
