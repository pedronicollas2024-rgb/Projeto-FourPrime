using FourPrime.Domain.Entities;

namespace FourPrime.Application.Abstractions.Repositories;

public interface IMarcaRepository
{
    Task<List<Marca>> GetAllAsync();
    Task<Marca?> GetByIdAsync(int id);
    Task AddAsync(Marca marca);
    Task DeleteAsync(int id);
    Task UpdateAsync(Marca marca);


}