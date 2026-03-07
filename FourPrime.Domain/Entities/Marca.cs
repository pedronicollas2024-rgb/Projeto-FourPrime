namespace FourPrime.Domain.Entities;

public class Marca
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string PaisOrigem { get; set; } = string.Empty;
    public int AnoFundacao { get; set; }
    public bool Ativo { get; set; } = true;


    // Navigation Property
    public virtual ICollection<Carro> Carros { get; set; } = new List<Carro>();
}