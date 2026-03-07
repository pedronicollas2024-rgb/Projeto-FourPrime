namespace FourPrime.Domain.Entities;

public class Carro
{
    public int Id { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public int Ano { get; set; }

    public string Cor { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Quilometragem { get; set; }
    public string Combustivel { get; set; } = string.Empty;

    // Novas propriedades adicionadas
    public string ImagemUrl { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool IsDestaque { get; set; } = false;
    public string? DestaqueTipo { get; set; } // "Blindados", "Esportivos", "Luxos"



    // Relacionamentos
    public int MarcaId { get; set; }
    public Marca Marca { get; set; } = null!;

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;
}