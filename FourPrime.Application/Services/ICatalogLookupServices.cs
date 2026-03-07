using FourPrime.Application.DTOs;

namespace FourPrime.Application.Services;

public interface ICatalogLookupService
{
    Task<List<MarcaDto>> GetMarcasAsync();
    Task<List<CategoriaDto>> GetCategoriasAsync();
}