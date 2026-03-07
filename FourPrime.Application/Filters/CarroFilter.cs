namespace FourPrime.Application.Filters;

public class CarroFilter
{
    public string? Modelo { get; set; }
    public int? MarcaId { get; set; }
    public int? CategoriaId { get; set; }
    public int? AnoMin { get; set; }
    public int? AnoMax { get; set; }
    public decimal? PrecoMin { get; set; }
    public decimal? PrecoMax { get; set; }
    public int? QuilometragemMax { get; set; }
    public string? Combustivel { get; set; }
    // ✅ Busca geral (Modelo, Cor, Marca, Categoria, Combustível...)
    public string? TermoPesquisa { get; set; }
    public string? DestaqueTipo { get; set; }
    public bool? IsDestaque { get; set; }
    public int? Take { get; set; }



    // Ordenação
    public string? SortBy { get; set; } = "Modelo";
    public bool Desc { get; set; } = false;
}