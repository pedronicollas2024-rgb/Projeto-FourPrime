using FourPrime.Application.DTOs;  // 🆕 ADICIONE ESTE USING
using System.Collections.Generic;
namespace FourPrime.Web.Models;


public class CatalogViewModel
{
    public List<CarroDto> Carros { get; set; } = new();
    public List<MarcaDto> Marcas { get; set; } = new();
    public List<CategoriaDto> Categorias { get; set; } = new();
    public CatalogFilterModel Filter { get; set; } = new();

    public List<CarroDto> Destaques { get; set; } = new();
    public List<CarroDto> DestaquesLoja { get; set; } = new();
}

public class CatalogFilterModel
{
    public string? TermoPesquisa { get; set; }
    public string? Modelo { get; set; } // ✅ MANTER esta propriedade também
    public int? MarcaId { get; set; }
    public int? CategoriaId { get; set; }
    public int? AnoMin { get; set; }
    public int? AnoMax { get; set; }
    public decimal? PrecoMin { get; set; }
    public decimal? PrecoMax { get; set; }
    public string? SortBy { get; set; }
    public bool Desc { get; set; }
    public string? PrecoRange { get; set; }

    public bool HasFilters =>
        !string.IsNullOrEmpty(TermoPesquisa) ||
        !string.IsNullOrEmpty(Modelo) ||
        MarcaId.HasValue ||
        CategoriaId.HasValue ||
        AnoMin.HasValue ||
        AnoMax.HasValue ||
        PrecoMin.HasValue ||
        PrecoMax.HasValue;
}