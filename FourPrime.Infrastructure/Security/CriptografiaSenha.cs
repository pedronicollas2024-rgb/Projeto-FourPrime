using FourPrime.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace FourPrime.Infrastructure.Security
{
    public class CriptografiaSenha : ICriptografiaSenha
    {
        public string GerarHash(string senhaEmTexto)
        {
            // SHA256 simples (ok pra estudo; em produção use BCrypt/Argon2/PBKDF2 com salt)
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senhaEmTexto);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        public bool Verificar(string senhaEmTexto, string hashArmazenado)
        {
            var novo = GerarHash(senhaEmTexto);
            return string.Equals(novo, hashArmazenado, StringComparison.OrdinalIgnoreCase);
        }
    }
}
