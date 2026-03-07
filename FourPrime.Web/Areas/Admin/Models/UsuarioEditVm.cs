using System.ComponentModel.DataAnnotations;

namespace FourPrime.Web.Areas.Admin.Models;

public class UsuarioEditVm
{
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome completo é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    [Display(Name = "Nome Completo")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    [Required(ErrorMessage = "A função é obrigatória.")]
    [Display(Name = "Função")]
    public string Role { get; set; } = string.Empty;

    // Para dropdown
    public List<string>? Roles { get; set; }
}