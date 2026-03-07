using FourPrime.Web.Models;

namespace FourPrime.Web.Services;

public interface IApiClient
{
    Task<List<CarroDto>> GetCarrosAsync(CatalogFilterModel filter);
    Task<CarroDto?> GetCarroByIdAsync(int id);
    Task<List<MarcaDto>> GetMarcasAsync();
    Task<List<CategoriaDto>> GetCategoriasAsync();
    Task<List<CarroDto>> GetDestaquesAsync(int take = 6);
    Task<List<CarroDto>> GetDestaquesLojaAsync(string tipo, int take = 8);


}
