using Microsoft.AspNetCore.Mvc;                    // 🎯 Biblioteca do ASP.NET Core para APIs
using FourPrime.Application.Services;             // 🎯 Onde estão os Services
using FourPrime.Application.DTOs;                 // 🎯 Onde estão os DTOs (dados leves)

namespace FourPrime.Api.Controllers;              // 🎯 Esta classe está na pasta API/Controllers

[ApiController]                                   // 🎯 MARCA: Esta é uma Controller de API
[Route("api/[controller]")]                       // 🎯 ROTA: Todas URLs começam com /api/marcas
public class MarcasController : ControllerBase    // 🎯 HERDA: ControllerBase (API)
{
    // 🎯 INJEÇÃO DE DEPENDÊNCIA: Recebe o Service de catálogo
    private readonly ICatalogLookupService _catalogService;

    // 🎯 CONSTRUTOR: Recebe o Service via injeção de dependência
    public MarcasController(ICatalogLookupService catalogService)
    {
        _catalogService = catalogService;  // 🎯 Guarda o Service para usar nos métodos
    }

    // 🎯 ENDPOINT 1: GET /api/marcas (lista TODAS as marcas)
    [HttpGet]  // 🎯 Responde a GET /api/marcas
    public async Task<ActionResult<List<MarcaDto>>> GetMarcas()
    {
        // 🎯 CHAMA o Service para buscar TODAS as marcas
        var marcas = await _catalogService.GetMarcasAsync();

        // 🎯 RETORNA: HTTP 200 OK + lista de marcas em JSON
        return Ok(marcas);
    }

    // 🎯 ENDPOINT 2: GET /api/marcas/5 (busca UMA marca específica)
    [HttpGet("{id}")]  // 🎯 Responde a GET /api/marcas/5
    public async Task<ActionResult<MarcaDto>> GetMarca(int id)
    {
        // 🚨 MÉTODO INEFICIENTE: Busca TODAS as marcas e filtra localmente
        var marcas = await _catalogService.GetMarcasAsync();

        // 🔍 Filtra para encontrar a marca com ID específico
        var marca = marcas.FirstOrDefault(m => m.Id == id);

        // 🎯 SE não encontrou → 404 Not Found
        if (marca == null)
        {
            return NotFound();  // 🎯 Retorna HTTP 404 sem mensagem
        }

        // 🎯 SE encontrou → 200 OK + dados da marca
        return Ok(marca);
    }
}