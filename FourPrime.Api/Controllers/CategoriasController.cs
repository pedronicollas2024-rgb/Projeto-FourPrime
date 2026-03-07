using Microsoft.AspNetCore.Mvc;                    // 🎯 Importa as classes do ASP.NET Core para criar controllers e ações
using FourPrime.Application.Services;             // 🎯 Importa os serviços da aplicação, onde está definido ICatalogLookupService
using FourPrime.Application.DTOs;                 // 🎯 Importa os DTOs (Data Transfer Objects) como CategoriaDto

namespace FourPrime.Api.Controllers;              // 🎯 Define que este controller está no namespace FourPrime.Api.Controllers

[ApiController]                                   // 🎯 Atributo que indica que esta classe é um controller de API
[Route("api/[controller]")]                       // 🎯 Define a rota base para este controller como /api/categorias
public class CategoriasController : ControllerBase // 🎯 Herda de ControllerBase, base para controllers de API
{
    // 🎯 Injeção de dependência: declara uma variável privada readonly para o serviço
    private readonly ICatalogLookupService _catalogService;

    // 🎯 Construtor: recebe o serviço por injeção de dependência e armazena na variável privada
    public CategoriasController(ICatalogLookupService catalogService)
    {
        _catalogService = catalogService;
    }

    // 🎯 Endpoint: GET /api/categorias
    [HttpGet] // 🎯 Mapeia este método para requisições GET na rota /api/categorias
    public async Task<ActionResult<List<CategoriaDto>>> GetCategorias()
    {
        // 🎯 Chama o serviço para obter a lista de categorias
        var categorias = await _catalogService.GetCategoriasAsync();
        // 🎯 Retorna a lista de categorias com status 200 OK
        return Ok(categorias);
    }

    // 🎯 Endpoint: GET /api/categorias/{id}
    [HttpGet("{id}")] // 🎯 Mapeia este método para requisições GET na rota /api/categorias/{id}
    public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
    {
        try
        {
            // 🎯 Obtém todas as categorias (nota: isso pode ser ineficiente para muitos dados)
            var categorias = await _catalogService.GetCategoriasAsync();
            // 🎯 Filtra a categoria pelo ID
            var categoria = categorias.FirstOrDefault(c => c.Id == id);

            // 🎯 Se não encontrou, retorna 404 Not Found
            if (categoria == null)
            {
                return NotFound($"Categoria com ID {id} não encontrada");
            }

            // 🎯 Se encontrou, retorna a categoria com status 200 OK
            return Ok(categoria);
        }
        catch (Exception ex)
        {
            // 🎯 Se ocorrer uma exceção, retorna 500 Internal Server Error com a mensagem
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }
}