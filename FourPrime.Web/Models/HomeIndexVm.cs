using FourPrime.Web.Models;

public class HomeIndexVm
{
    public List<CarroDto> Destaques { get; set; } = new();
    public List<MarcaDto> Marcas { get; set; } = new();
    public List<CategoriaDto> Categorias { get; set; } = new();
    public CatalogFilterModel Filter { get; set; } = new();
}
