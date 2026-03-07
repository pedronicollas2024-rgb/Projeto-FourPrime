namespace FourPrime.Application.Interfaces
{
    public interface ICriptografiaSenha
    {
        string GerarHash(string senhaEmTexto);
        bool Verificar(string senhaEmTexto, string hashArmazenado);
    }
}
