using FourPrime.Application.Abstractions.Repositories;
using FourPrime.Domain.Entities;
using FourPrime.Infrastructure.Database;
using FourPrime.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FourPrime.Infrastructure.Repositories;

public class MarcaRepository : IMarcaRepository
{
    private readonly AppDbContext _context;

   

    public MarcaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Marca>> GetAllAsync()
    {
        return await _context.Set<Marca>()
            .AsNoTracking()
            .OrderBy(m => m.Nome)
            .ToListAsync();
    }

    public async Task AddAsync(Marca marca)
    {
        _context.Set<Marca>().Add(marca);
        await _context.SaveChangesAsync();
    }
    public async Task<Marca?> GetByIdAsync(int id) =>
    await _context.Set<Marca>()
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Id == id);
    public async Task DeleteAsync(int id)
    {
        var marca = await _context.Set<Marca>().FindAsync(id);
        if (marca is null) return;

        _context.Set<Marca>().Remove(marca);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Marca marca)
    {
        _context.Set<Marca>().Update(marca);
        await _context.SaveChangesAsync();
    }

}