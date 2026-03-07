using FourPrime.Application.Services;
using FourPrime.Application.Abstractions.Repositories;
using FourPrime.Application.DTOs;
using FourPrime.Domain.Entities;

namespace FourPrime.Infrastructure.Services;

public class CatalogLookupService : ICatalogLookupService
{
    private readonly IMarcaRepository _marcaRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public CatalogLookupService(IMarcaRepository marcaRepository, ICategoriaRepository categoriaRepository)
    {
        _marcaRepository = marcaRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<List<MarcaDto>> GetMarcasAsync()
    {
        var marcas = await _marcaRepository.GetAllAsync();
        return marcas.Select(m => new MarcaDto
        {
            Id = m.Id,
            Nome = m.Nome,
            PaisOrigem = m.PaisOrigem,
            AnoFundacao = m.AnoFundacao
        }).ToList();
    }

    public async Task<List<CategoriaDto>> GetCategoriasAsync()
    {
        var categorias = await _categoriaRepository.GetAllAsync();
        return categorias.Select(c => new CategoriaDto
        {
            Id = c.Id,
            Nome = c.Nome,
            Descricao = c.Descricao
        }).ToList();
    }
}