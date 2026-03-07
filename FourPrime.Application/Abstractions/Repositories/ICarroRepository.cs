using FourPrime.Domain.Entities;
using FourPrime.Application.Filters;
using FourPrime.Application.DTOs;

namespace FourPrime.Application.Abstractions.Repositories;

public interface ICarroRepository
{
    Task<List<Carro>> GetByFilterAsync(CarroFilter filter);
    Task<Carro?> GetByIdAsync(int id);
    Task<List<Carro>> GetAllAsync();
    Task AddAsync(Carro carro);
    Task UpdateAsync(Carro carro);
    Task DeleteAsync(int id);
}