using Microsoft.AspNetCore.Mvc;                    // 🎯 Biblioteca do ASP.NET Core para APIs
using FourPrime.Application.Services;             // 🎯 Onde estão os Services
using FourPrime.Application.DTOs;                 // 🎯 Onde estão os DTOs (dados leves)
using FourPrime.Application.Filters;              // 🎯 Onde estão os Filtros (buscas)

namespace FourPrime.Api.Controllers;              // 🎯 Esta classe está na pasta API/Controllers

[ApiController]                                   // 🎯 MARCA: Esta é uma Controller de API
[Route("api/[controller]")]                       // 🎯 ROTA: Todas URLs começam com /api/carros
public class CarrosController : ControllerBase    // 🎯 HERDA: ControllerBase (API) em vez de Controller (MVC)
{
    // 🎯 INJEÇÃO DE DEPENDÊNCIA: Recebe o Service via construtor
    private readonly ICarroQueryService _carroQueryService;

    // 🎯 CONSTRUTOR: O ASP.NET injeta automaticamente o Service
    public CarrosController(ICarroQueryService carroQueryService)
    {
        _carroQueryService = carroQueryService;  // 🎯 Guarda o Service para usar nos métodos
    }


    [HttpGet]  // 🎯 Responde a GET /api/carros
    public async Task<ActionResult<List<CarroDto>>> GetCarros([FromQuery] CarroFilter filter)
    {
        // 🎯 CHAMA o Service para buscar carros com filtros
        var carros = await _carroQueryService.GetCarrosAsync(filter);

        // 🎯 RETORNA: HTTP 200 OK + lista de carros em JSON
        return Ok(carros);
    }

    [HttpGet("{id}")]  // 🎯 Responde a GET /api/carros/5 (onde 5 é o ID)
    public async Task<ActionResult<CarroDto>> GetCarro(int id)
    {
        // 🎯 CHAMA o Service para buscar UM carro específico
        var carro = await _carroQueryService.GetCarroByIdAsync(id);

        // 🎯 SE não encontrou → 404 Not Found
        // 🎯 SE encontrou → 200 OK + dados do carro
        return carro == null ? NotFound() : Ok(carro);
    }
    // GET /api/carros/destaques-loja?tipo=Esportivos&take=8
    [HttpGet("destaques-loja")]
    public async Task<ActionResult<List<CarroDto>>> GetDestaquesLoja(
        [FromQuery] string? tipo,
        [FromQuery] int take = 8)
    {
        var filter = new CarroFilter
        {
            // ✅ NÃO amarra mais no IsDestaque
            // IsDestaque = true,

            DestaqueTipo = tipo,
            Take = take,

            // opcional: uma ordenação melhor para vitrine (se você quiser)
            // SortBy = "Ano",
            // Desc = true
        };

        var carros = await _carroQueryService.GetCarrosAsync(filter);
        return Ok(carros);
    }

    // GET /api/carros/destaques?take=6
    [HttpGet("destaques")]
    public async Task<ActionResult<List<CarroDto>>> GetDestaques([FromQuery] int take = 6)
    {
        var filter = new CarroFilter
        {
            IsDestaque = true,
            Take = take
        };

        var carros = await _carroQueryService.GetCarrosAsync(filter);
        return Ok(carros);
    }





}