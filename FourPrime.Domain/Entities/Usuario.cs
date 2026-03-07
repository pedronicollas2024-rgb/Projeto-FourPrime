using System;

namespace FourPrime.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; set; }

        // Identificação
        public string NomeCompleto { get; set; } = string.Empty;
        public string NomeDeUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;

        // Credencial (NUNCA guardar senha em texto puro)
        public string HashSenha { get; set; } = string.Empty;

        // Status
        public bool Ativo { get; set; } = true;
        public bool EmailVerificado { get; set; } = false;

        // Preferência de login
        public bool LembrarMePadrao { get; set; } = false;

        // Segurança/Auditoria
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public DateTime? AtualizadoEm { get; set; }
        public DateTime? UltimoLoginEm { get; set; }

        // Anti brute-force (opcional)
        public int TentativasLoginFalhas { get; set; } = 0;
        public DateTime? BloqueadoAte { get; set; }

        // Perfil simples (ex.: "Cliente", "Admin", "Funcionario")
        public string Perfil { get; set; } = "Cliente";
    }
}
