using System.ComponentModel.DataAnnotations;
using FourPrime.Domain.Entities;

namespace FourPrime.Web.Areas.Admin.Models;

public class CarroEditVm
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O modelo é obrigatório.")]
    [StringLength(100, ErrorMessage = "O modelo deve ter no máximo 100 caracteres.")]
    [Display(Name = "Modelo")]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O ano é obrigatório.")]
    [Range(1900, 2024, ErrorMessage = "O ano deve estar entre 1900 e 2024.")]
    [Display(Name = "Ano")]
    public int Ano { get; set; }

    [Required(ErrorMessage = "O preço é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
    [Display(Name = "Preço")]
    public decimal Preco { get; set; }

    [Required(ErrorMessage = "A marca é obrigatória.")]
    [Display(Name = "Marca")]
    public int MarcaId { get; set; }

    [Required(ErrorMessage = "A categoria é obrigatória.")]
    [Display(Name = "Categoria")]
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "A cor é obrigatória.")]
    [StringLength(50, ErrorMessage = "A cor deve ter no máximo 50 caracteres.")]
    [Display(Name = "Cor")]
    public string Cor { get; set; } = string.Empty;

    [Required(ErrorMessage = "A quilometragem é obrigatória.")]
    [Range(0, int.MaxValue, ErrorMessage = "A quilometragem não pode ser negativa.")]
    [Display(Name = "Quilometragem")]
    public int Quilometragem { get; set; }

    [Required(ErrorMessage = "O combustível é obrigatório.")]
    [StringLength(30, ErrorMessage = "O combustível deve ter no máximo 30 caracteres.")]
    [Display(Name = "Combustível")]
    public string Combustivel { get; set; } = string.Empty; // ✅ ADICIONADO

    [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres.")]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = string.Empty;

    [Display(Name = "Imagem do Carro")]
    public IFormFile? ImagemFile { get; set; }

    [Display(Name = "Imagem Atual")]
    public string? ImagemUrl { get; set; }

    // Para dropdowns
    public List<Marca>? Marcas { get; set; }
    public List<Categoria>? Categorias { get; set; }
    public bool IsDestaque { get; set; }
    public string? DestaqueTipo { get; set; }



}