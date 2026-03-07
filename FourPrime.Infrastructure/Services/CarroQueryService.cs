using Microsoft.EntityFrameworkCore;
using FourPrime.Application.Services;
using FourPrime.Application.DTOs;
using FourPrime.Application.Filters;
using FourPrime.Infrastructure.Persistence;
using FourPrime.Domain.Entities;

namespace FourPrime.Infrastructure.Services;

public class CarroQueryService : ICarroQueryService
{
    private readonly AppDbContext _context;

    public CarroQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CarroDto>> GetCarrosAsync(CarroFilter filter)
    {
        var query = _context.Carros
            .Include(c => c.Marca)
            .Include(c => c.Categoria)
            .AsQueryable();

        // ✅ Busca geral (termoPesquisa) - opcional, mas você já tem no filtro
        if (!string.IsNullOrWhiteSpace(filter.TermoPesquisa))
        {
            var termo = filter.TermoPesquisa.Trim();

            query = query.Where(c =>
                c.Modelo.Contains(termo) ||
                c.Cor.Contains(termo) ||
                c.Combustivel.Contains(termo) ||
                c.Marca.Nome.Contains(termo) ||
                c.Categoria.Nome.Contains(termo)
            );
        }

        // Aplicar filtros
        if (!string.IsNullOrEmpty(filter.Modelo))
            query = query.Where(c => c.Modelo.Contains(filter.Modelo));

        if (filter.MarcaId.HasValue)
            query = query.Where(c => c.MarcaId == filter.MarcaId.Value);

        if (filter.CategoriaId.HasValue)
            query = query.Where(c => c.CategoriaId == filter.CategoriaId.Value);

        if (filter.AnoMin.HasValue)
            query = query.Where(c => c.Ano >= filter.AnoMin.Value);

        if (filter.AnoMax.HasValue)
            query = query.Where(c => c.Ano <= filter.AnoMax.Value);

        if (filter.PrecoMin.HasValue)
            query = query.Where(c => c.Preco >= filter.PrecoMin.Value);

        if (filter.PrecoMax.HasValue)
            query = query.Where(c => c.Preco <= filter.PrecoMax.Value);

        // Filtro por Quilometragem
        if (filter.QuilometragemMax.HasValue)
            query = query.Where(c => c.Quilometragem <= filter.QuilometragemMax.Value);

        // Filtro por Combustível
        if (!string.IsNullOrEmpty(filter.Combustivel))
            query = query.Where(c => c.Combustivel == filter.Combustivel);

    
        // Aplicar ordenação
        query = ApplySorting(query, filter.SortBy, filter.Desc);

        if (filter.IsDestaque.HasValue)
            query = query.Where(c => c.IsDestaque == filter.IsDestaque.Value);

        if (!string.IsNullOrWhiteSpace(filter.DestaqueTipo))
            query = query.Where(c => c.DestaqueTipo == filter.DestaqueTipo);

        if (filter.Take.HasValue && filter.Take.Value > 0)
            query = query.Take(filter.Take.Value);



        var carros = await query.ToListAsync();
        return carros.Select(MapToDto).ToList();
    }

    public async Task<CarroDto?> GetCarroByIdAsync(int id)
    {
        var carro = await _context.Carros
            .Include(c => c.Marca)
            .Include(c => c.Categoria)
            .FirstOrDefaultAsync(c => c.Id == id);

        return carro == null ? null : MapToDto(carro);
    }

    public async Task<List<CarroDto>> GetCarrosPorMarcaAsync(int marcaId)
    {
        var carros = await _context.Carros
            .Include(c => c.Marca)
            .Include(c => c.Categoria)
            .Where(c => c.MarcaId == marcaId)
            .ToListAsync();

        return carros.Select(MapToDto).ToList();
    }

    public async Task<List<CarroDto>> GetCarrosPorCategoriaAsync(int categoriaId)
    {
        var carros = await _context.Carros
            .Include(c => c.Marca)
            .Include(c => c.Categoria)
            .Where(c => c.CategoriaId == categoriaId)
            .ToListAsync();

        return carros.Select(MapToDto).ToList();
    }

    public async Task<List<CarroDto>> GetCarrosPorPrecoAsync(decimal precoMin, decimal precoMax)
    {
        var carros = await _context.Carros
            .Include(c => c.Marca)
            .Include(c => c.Categoria)
            .Where(c => c.Preco >= precoMin && c.Preco <= precoMax)
            .ToListAsync();

        return carros.Select(MapToDto).ToList();
    }

    public async Task<List<CarroDto>> GetCarrosPorAnoAsync(int anoMin, int anoMax)
    {
        var carros = await _context.Carros
            .Include(c => c.Marca)
            .Include(c => c.Categoria)
            .Where(c => c.Ano >= anoMin && c.Ano <= anoMax)
            .ToListAsync();

        return carros.Select(MapToDto).ToList();
    }

    public async Task<List<CarroDto>> GetCarrosRecentesAsync(int quantidade = 10)
    {
        var carros = await _context.Carros
            .Include(c => c.Marca)
            .Include(c => c.Categoria)
            .OrderByDescending(c => c.Ano)
            .Take(quantidade)
            .ToListAsync();

        return carros.Select(MapToDto).ToList();
    }

    public async Task<decimal> GetPrecoMedioPorMarcaAsync(int marcaId)
    {
        var precoMedio = await _context.Carros
            .Where(c => c.MarcaId == marcaId)
            .AverageAsync(c => c.Preco);

        return precoMedio;
    }

    public async Task<int> GetTotalCarrosAsync()
    {
        return await _context.Carros.CountAsync();
    }

    // Método para filtrar por combustível
    public async Task<List<CarroDto>> GetCarrosPorCombustivelAsync(string combustivel)
    {
        var carros = await _context.Carros
            .Include(c => c.Marca)
            .Include(c => c.Categoria)
            .Where(c => c.Combustivel == combustivel)
            .ToListAsync();

        return carros.Select(MapToDto).ToList();
    }

    // Método auxiliar para ordenação
    private static IQueryable<Carro> ApplySorting(IQueryable<Carro> query, string? sortBy, bool desc)
    {
        return sortBy?.ToLower() switch
        {
            "preco" => desc ? query.OrderByDescending(c => c.Preco) : query.OrderBy(c => c.Preco),
            "ano" => desc ? query.OrderByDescending(c => c.Ano) : query.OrderBy(c => c.Ano),
            "quilometragem" => desc ? query.OrderByDescending(c => c.Quilometragem) : query.OrderBy(c => c.Quilometragem),
            "modelo" => desc ? query.OrderByDescending(c => c.Modelo) : query.OrderBy(c => c.Modelo),
            "marca" => desc ? query.OrderByDescending(c => c.Marca.Nome) : query.OrderBy(c => c.Marca.Nome),
            "categoria" => desc ? query.OrderByDescending(c => c.Categoria.Nome) : query.OrderBy(c => c.Categoria.Nome),
            _ => desc ? query.OrderByDescending(c => c.Modelo) : query.OrderBy(c => c.Modelo)
        };
    }

    // Método auxiliar para mapear Carro para CarroDto
    private static CarroDto MapToDto(Carro carro)
    {
        return new CarroDto
        {
            Id = carro.Id,
            Modelo = carro.Modelo,
            Ano = carro.Ano,
            Cor = carro.Cor,
            Preco = carro.Preco,
            Quilometragem = carro.Quilometragem,
            Combustivel = carro.Combustivel,
            ImagemUrl = carro.ImagemUrl,
            Descricao = carro.Descricao,
            Marca = carro.Marca?.Nome ?? string.Empty,
            Categoria = carro.Categoria?.Nome ?? string.Empty,
            IsDestaque = carro.IsDestaque,
            DestaqueTipo = carro.DestaqueTipo

        };
    }
}
