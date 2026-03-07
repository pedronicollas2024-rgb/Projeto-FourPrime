namespace FourPrime.Application.DTOs
{
    public class LoginRequest
    {
        public string UsuarioOuEmail { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public bool LembrarMe { get; set; } = false;
    }
}
