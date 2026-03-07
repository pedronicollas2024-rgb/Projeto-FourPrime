using FourPrime.Infrastructure.Persistence;
using FourPrime.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FourPrime.Web.Areas.Admin.Services;

public class DashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardData> GetDashboardDataAsync()
    {
        return new DashboardData
        {
            TotalCarros = await _context.Carros.CountAsync(),
            TotalMarcas = await _context.Marcas.CountAsync(),
            TotalCategorias = await _context.Categorias.CountAsync(),
            TotalUsuarios = 0, // ✅ SEM UserManager - podemos deixar 0 ou contar de outra forma

            CarrosPorMarca = await _context.Carros
                .Include(c => c.Marca)
                .GroupBy(c => c.Marca.Nome)
                .Select(g => new { Marca = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Marca, x => x.Count),

            CarrosPorCategoria = await _context.Carros
                .Include(c => c.Categoria)
                .GroupBy(c => c.Categoria.Nome)
                .Select(g => new { Categoria = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Categoria, x => x.Count),

            UltimosCarros = await _context.Carros
                .Include(c => c.Marca)
                .Include(c => c.Categoria)
                .OrderByDescending(c => c.Id)
                .Take(5)
                .Select(c => new CarroDto
                {
                    Id = c.Id,
                    Modelo = c.Modelo,
                    Marca = c.Marca.Nome,
                    Categoria = c.Categoria.Nome,
                    Ano = c.Ano,
                    Preco = c.Preco,
                    Cor = c.Cor,
                    Quilometragem = c.Quilometragem,
                    Combustivel = c.Combustivel,
                    ImagemUrl = c.ImagemUrl,
                    Descricao = c.Descricao,
                    IsDestaque = c.IsDestaque,

                })
                .ToListAsync()
        };
    }
}

public class DashboardData
{
    public int TotalCarros { get; set; }
    public int TotalMarcas { get; set; }
    public int TotalCategorias { get; set; }
    public int TotalUsuarios { get; set; }
    public Dictionary<string, int> CarrosPorMarca { get; set; } = new();
    public Dictionary<string, int> CarrosPorCategoria { get; set; } = new();
    public List<CarroDto> UltimosCarros { get; set; } = new();
}