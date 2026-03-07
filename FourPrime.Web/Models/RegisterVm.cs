using System.ComponentModel.DataAnnotations;

namespace FourPrime.Web.Models;

public class RegisterVm
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    public string NomeCompleto { get; set; } = "";

    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Confirme a senha")]
    [Compare("Password", ErrorMessage = "As senhas não conferem")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = "";
}
