using System;

namespace FourPrime.Application.DTOs
{
    public class LoginResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;

        public Guid? UsuarioId { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string Perfil { get; set; } = string.Empty;

        public string TokenSessao { get; set; } = string.Empty;
        public DateTime? ExpiraEm { get; set; }
    }
}
