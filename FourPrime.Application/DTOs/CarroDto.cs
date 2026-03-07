namespace FourPrime.Application.DTOs;

public class CarroDto
{
    public int Id { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public int Ano { get; set; }
    public string Cor { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Quilometragem { get; set; }
    public string Combustivel { get; set; } = string.Empty;
    public string ImagemUrl { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;

    // Pelo seu DTO atual, Marca/Categoria estão como string
    public string Marca { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;

    // ✅ NOVO: campo para controlar se o carro aparece na seção de destaque
    public bool IsDestaque { get; set; }
    public string? DestaqueTipo { get; set; }

}
