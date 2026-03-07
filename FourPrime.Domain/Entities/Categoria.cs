using System.Collections.Generic;

namespace FourPrime.Domain.Entities; 

public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;

    // PROPRIEDADE QUE RESOLVE O ERRO
    public virtual ICollection<Carro> Carros { get; set; } = new List<Carro>();
}