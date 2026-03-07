using System;

namespace FourPrime.Domain.Entities
{
    public class TokenRecuperacaoSenha
    {
        public Guid Id { get; set; }

        public Guid UsuarioId { get; set; }

        // Token enviado (link/código)
        public string Token { get; set; } = string.Empty;

        // Controle de validade
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public DateTime ExpiraEm { get; set; }

        // Controle de uso
        public bool Usado { get; set; } = false;
        public DateTime? UsadoEm { get; set; }

        // Opcional: motivo/canal
        public string Canal { get; set; } = "Email"; // Email, SMS, WhatsApp...
    }
}
