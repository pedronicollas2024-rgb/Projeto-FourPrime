namespace FourPrime.Application.Filters;

public class MarcaFilter
{
    public string? Nome { get; set; }
    public string? PaisOrigem { get; set; }
    public int? AnoFundacaoMin { get; set; }
    public int? AnoFundacaoMax { get; set; }
}