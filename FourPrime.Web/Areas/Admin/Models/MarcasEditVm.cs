using System.ComponentModel.DataAnnotations;

namespace FourPrime.Web.Areas.Admin.Models;

public class MarcaEditVm
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da marca é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    [Display(Name = "Nome da Marca")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O país de origem é obrigatório.")]
    [StringLength(50, ErrorMessage = "O país deve ter no máximo 50 caracteres.")]
    [Display(Name = "País de Origem")]
    public string PaisOrigem { get; set; } = string.Empty;

    [Required(ErrorMessage = "O ano de fundação é obrigatório.")]
    [Range(1800, 2024, ErrorMessage = "O ano de fundação deve estar entre 1800 e 2024.")]
    [Display(Name = "Ano de Fundação")]
    public int AnoFundacao { get; set; }

}