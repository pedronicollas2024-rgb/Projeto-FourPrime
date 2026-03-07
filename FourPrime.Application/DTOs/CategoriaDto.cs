namespace FourPrime.Application.DTOs;

public class CategoriaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    // NÃO inclua a lista de Carros aqui
}