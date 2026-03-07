using FourPrime.Application.DTOs;
using FourPrime.Application.Filters;

public interface ICarroQueryService
{
    Task<List<CarroDto>> GetCarrosAsync(CarroFilter filter);
    Task<CarroDto?> GetCarroByIdAsync(int id);
    Task<List<CarroDto>> GetCarrosPorMarcaAsync(int marcaId);
    Task<List<CarroDto>> GetCarrosPorCategoriaAsync(int categoriaId);
    Task<List<CarroDto>> GetCarrosPorPrecoAsync(decimal precoMin, decimal precoMax);
    Task<List<CarroDto>> GetCarrosPorAnoAsync(int anoMin, int anoMax);
    Task<List<CarroDto>> GetCarrosRecentesAsync(int quantidade = 10);
    Task<decimal> GetPrecoMedioPorMarcaAsync(int marcaId);
    Task<int> GetTotalCarrosAsync();
    Task<List<CarroDto>> GetCarrosPorCombustivelAsync(string combustivel); // Adicione esta linha
}