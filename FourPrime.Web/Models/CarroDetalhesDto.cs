using FourPrime.Application.DTOs;

namespace FourPrime.Web.Models;

public class CarroDetalhesDto : CarroDto
{
    public List<string> Imagens { get; set; } = new();
}
