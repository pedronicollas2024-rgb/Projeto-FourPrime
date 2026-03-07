using FourPrime.Application.Abstractions.Repositories;
using FourPrime.Domain.Entities;
using FourPrime.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FourPrime.Infrastructure.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _context;

    public CategoriaRepository(AppDbContext context) => _context = context;

    public async Task<List<Categoria>> GetAllAsync() =>
        await _context.Set<Categoria>()
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .ToListAsync();

    public async Task<Categoria?> GetByIdAsync(int id) =>
        await _context.Set<Categoria>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task AddAsync(Categoria categoria)
    {
        _context.Set<Categoria>().Add(categoria);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Categoria categoria)
    {
        _context.Set<Categoria>().Update(categoria);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var categoria = await _context.Set<Categoria>().FindAsync(id);
        if (categoria is null) return;

        _context.Set<Categoria>().Remove(categoria);
        await _context.SaveChangesAsync();
    }
}