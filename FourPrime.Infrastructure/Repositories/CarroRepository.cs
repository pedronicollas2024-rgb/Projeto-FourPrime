using FourPrime.Application.Abstractions.Repositories;
using FourPrime.Domain.Entities;
using FourPrime.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FourPrime.Application.Filters;

namespace FourPrime.Infrastructure.Repositories;

public class CarroRepository : ICarroRepository
{
    private readonly AppDbContext _context;

    public CarroRepository(AppDbContext context) => _context = context;


    public async Task<List<Carro>> GetByFilterAsync(CarroFilter filter)
    {
        var query = _context.Carros
            .AsNoTracking()
            .Include(c => c.Marca)
            .Include(c => c.Categoria)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Modelo))
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

        // Ordenação básica (opcional)
        query = query.OrderByDescending(c => c.Id);

        return await query.ToListAsync();
    }
    public async Task<List<Carro>> GetAllAsync()
    {
        return await _context.Carros
            .AsNoTracking()
            .Include(c => c.Marca)
            .Include(c => c.Categoria)
            .OrderByDescending(c => c.Id)
            .ToListAsync();
    }

    public async Task<Carro?> GetByIdAsync(int id)
    {
        return await _context.Carros
            .AsNoTracking()
            .Include(c => c.Marca)
            .Include(c => c.Categoria)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Carro carro)
    {
        _context.Carros.Add(carro);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(int id)
    {
        var carro = await _context.Set<Carro>().FindAsync(id);
        if (carro is null) return;

        _context.Set<Carro>().Remove(carro);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(Carro carro)
    {
        var existente = await _context.Set<Carro>().FindAsync(carro.Id);
        if (existente is null) return;

        // Atualiza só o que você edita no form
        existente.Modelo = carro.Modelo;
        existente.Ano = carro.Ano;
        existente.Preco = carro.Preco;
        existente.MarcaId = carro.MarcaId;
        existente.CategoriaId = carro.CategoriaId;

        await _context.SaveChangesAsync();
    }
}