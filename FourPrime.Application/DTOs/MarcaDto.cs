namespace FourPrime.Application.DTOs;

public class MarcaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string PaisOrigem { get; set; } = string.Empty;
    public int AnoFundacao { get; set; }
}