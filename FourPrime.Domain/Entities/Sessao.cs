using System;

namespace FourPrime.Domain.Entities
{
    public class Sessao
    {
        public Guid Id { get; set; }

        public Guid UsuarioId { get; set; }

        // Token da sessão (se você for armazenar sessão no banco)
        public string TokenSessao { get; set; } = string.Empty;

        // Controle
        public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
        public DateTime ExpiraEm { get; set; }
        public bool Persistente { get; set; } = false;

        // Telemetria simples (opcional)
        public string Ip { get; set; } = string.Empty;
        public string AgenteUsuario { get; set; } = string.Empty; // browser/app

        // Encerramento
        public bool Ativa { get; set; } = true;
        public DateTime? EncerradaEm { get; set; }
    }
}
