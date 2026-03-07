namespace FourPrime.Web.Models;

public class CarroDto
{
    public int Id { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public int Ano { get; set; }
    public string Cor { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Quilometragem { get; set; }
    public string Combustivel { get; set; } = string.Empty;
    public string ImagemUrl { get; set; } = string.Empty;  // ✅ TEM ImagemUrl
    public string Descricao { get; set; } = string.Empty;  // ✅ TEM Descricao
    public string Marca { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string? DestaqueTipo { get; set; }
    public bool IsDestaque { get; set; }
}
public class MarcaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string PaisOrigem { get; set; } = string.Empty;
}

public class CategoriaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
}