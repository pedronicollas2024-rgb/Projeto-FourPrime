using Microsoft.AspNetCore.Mvc;
using FourPrime.Web.Models;
using FourPrime.Web.Services;

namespace FourPrime.Web.Controllers;

public class HomeController : Controller
{
    private readonly IApiClient _apiClient;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IApiClient apiClient, ILogger<HomeController> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    // ==========================
    // HOME
    // ==========================
    public async Task<IActionResult> Index()
    {
        try
        {
            var filter = new CatalogFilterModel
            {
                SortBy = "Ano",
                Desc = true
            };

            // ✅ Destaques da Loja (por tipo)
            var blindadosTask = _apiClient.GetDestaquesLojaAsync("Blindados", 12);
            var esportivosTask = _apiClient.GetDestaquesLojaAsync("Esportivos", 12);
            var luxosTask = _apiClient.GetDestaquesLojaAsync("Luxos", 12);

            await Task.WhenAll(blindadosTask, esportivosTask, luxosTask);

            var destaquesLoja = new List<CarroDto>();
            destaquesLoja.AddRange(blindadosTask.Result);
            destaquesLoja.AddRange(esportivosTask.Result);
            destaquesLoja.AddRange(luxosTask.Result);

            var model = new CatalogViewModel
            {
                Marcas = await _apiClient.GetMarcasAsync(),
                Categorias = await _apiClient.GetCategoriasAsync(),
                Carros = await _apiClient.GetCarrosAsync(filter),
                Filter = filter,

                // ✅ destaques do topo (isDestaque)
                Destaques = await _apiClient.GetDestaquesAsync(6),

                // ✅ destaques da loja (por tipo)
                DestaquesLoja = destaquesLoja
            };

            _logger.LogInformation(
                "DEBUG INDEX: Carros={Carros} | Destaques={Destaques} | DestaquesLoja={DestaquesLoja} | Marcas={Marcas} | Categorias={Categorias}",
                model.Carros?.Count ?? -1,
                model.Destaques?.Count ?? -1,
                model.DestaquesLoja?.Count ?? -1,
                model.Marcas?.Count ?? -1,
                model.Categorias?.Count ?? -1
            );

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar página inicial");
            return View(new CatalogViewModel());
        }
    }

    [HttpPost]
    public async Task<IActionResult> Index(CatalogFilterModel filter)
    {
        try
        {
            // ✅ mantém destaques da loja também no POST
            var blindadosTask = _apiClient.GetDestaquesLojaAsync("Blindados", 12);
            var esportivosTask = _apiClient.GetDestaquesLojaAsync("Esportivos", 12);
            var luxosTask = _apiClient.GetDestaquesLojaAsync("Luxos", 12);

            await Task.WhenAll(blindadosTask, esportivosTask, luxosTask);

            var destaquesLoja = new List<CarroDto>();
            destaquesLoja.AddRange(blindadosTask.Result);
            destaquesLoja.AddRange(esportivosTask.Result);
            destaquesLoja.AddRange(luxosTask.Result);

            var model = new CatalogViewModel
            {
                Carros = await _apiClient.GetCarrosAsync(filter),
                Marcas = await _apiClient.GetMarcasAsync(),
                Categorias = await _apiClient.GetCategoriasAsync(),
                Filter = filter,

                Destaques = await _apiClient.GetDestaquesAsync(6),
                DestaquesLoja = destaquesLoja
            };

            _logger.LogInformation(
                "DEBUG INDEX POST: Carros={Carros} | Destaques={Destaques} | DestaquesLoja={DestaquesLoja}",
                model.Carros?.Count ?? -1,
                model.Destaques?.Count ?? -1,
                model.DestaquesLoja?.Count ?? -1
            );

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao filtrar carros");
            return View(new CatalogViewModel { Filter = filter });
        }
    }

    public IActionResult About() => View();
    public IActionResult Contact() => View();

    // ==========================
    // CATALOGO (PROTEGIDO POR SESSION)
    // ==========================
    public async Task<IActionResult> Catalog()
    {
        if (HttpContext.Session.GetString("UsuarioLogado") != "true")
            return RedirectToAction("Login", "Auth", new { returnUrl = "/Home/Catalog" });

        try
        {
            var model = new CatalogViewModel
            {
                Carros = await _apiClient.GetCarrosAsync(new CatalogFilterModel()),
                Marcas = await _apiClient.GetMarcasAsync(),
                Categorias = await _apiClient.GetCategoriasAsync(),
                Destaques = await _apiClient.GetDestaquesAsync(6)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar catálogo");
            return View(new CatalogViewModel());
        }
    }

    // ==========================
    // ADMIN (REDIRECIONA PARA AREA ADMIN)
    // ==========================
    public IActionResult Admin()
    {
        if (HttpContext.Session.GetString("UsuarioLogado") != "true")
            return RedirectToAction("Login", "Auth", new { returnUrl = "/Home/Admin" });

        var userRole = HttpContext.Session.GetString("UsuarioRole");
        if (userRole != "Admin" && userRole != "Manager")
            return RedirectToAction("AccessDenied", "Auth");

        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();

    // ==========================
    // RESULTADOS (FILTRO GET)
    // ==========================
    public async Task<IActionResult> Resultados(CatalogFilterModel filter)
    {
        try
        {
            var blindadosTask = _apiClient.GetDestaquesLojaAsync("Blindados", 12);
            var esportivosTask = _apiClient.GetDestaquesLojaAsync("Esportivos", 12);
            var luxosTask = _apiClient.GetDestaquesLojaAsync("Luxos", 12);

            await Task.WhenAll(blindadosTask, esportivosTask, luxosTask);

            var destaquesLoja = new List<CarroDto>();
            destaquesLoja.AddRange(blindadosTask.Result);
            destaquesLoja.AddRange(esportivosTask.Result);
            destaquesLoja.AddRange(luxosTask.Result);

            var model = new CatalogViewModel
            {
                Carros = await _apiClient.GetCarrosAsync(filter),
                Marcas = await _apiClient.GetMarcasAsync(),
                Categorias = await _apiClient.GetCategoriasAsync(),
                Filter = filter,
                Destaques = await _apiClient.GetDestaquesAsync(6),
                DestaquesLoja = destaquesLoja
            };

            _logger.LogInformation(
                "DEBUG RESULTADOS: Carros={Carros}, Destaques={Destaques}, DestaquesLoja={DestaquesLoja}",
                model.Carros?.Count ?? -1,
                model.Destaques?.Count ?? -1,
                model.DestaquesLoja?.Count ?? -1
            );

            return View("Resultados", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar carros");
            return View("Resultados", new CatalogViewModel { Filter = filter });
        }
    }

    // ==========================
    // PÁGINAS FIXAS DE CARROS
    // ==========================
    public IActionResult FerrariSpider() => View("~/Views/Home/Carros/FerrariSpider.cshtml");
    public IActionResult LAMBORGHINIR() => View("~/Views/Home/Carros/LAMBORGHINIR.cshtml");
    public IActionResult Porsche911V() => View("~/Views/Home/Carros/Porsche911V.cshtml");
    public IActionResult Mustang() => View("~/Views/Home/Carros/Mustang.cshtml");
    public IActionResult MercedezC300() => View("~/Views/Home/Carros/MercedezC300.cshtml");
    public IActionResult BmwM2() => View("~/Views/Home/Carros/BmwM2.cshtml");
    public IActionResult PorscheC() => View("~/Views/Home/Carros/PorscheC.cshtml");
    public IActionResult FerrariPuroS() => View("~/Views/Home/Carros/FerrariPuroS.cshtml");
    public IActionResult Lambourus() => View("~/Views/Home/Carros/Lambourus.cshtml");
    public IActionResult Porsche911A() => View("~/Views/Home/Carros/Porsche911A.cshtml");

    // ==========================
    // DETALHES DINÂMICO
    // ==========================
    public async Task<IActionResult> Detalhes(int id)
    {
        try
        {
            var carro = await _apiClient.GetCarroByIdAsync(id);
            if (carro == null) return NotFound();

            var detalhes = new CarroDetalhesDto
            {
                Id = carro.Id,
                Modelo = carro.Modelo,
                Ano = carro.Ano,
                Cor = carro.Cor,
                Preco = carro.Preco,
                Quilometragem = carro.Quilometragem,
                Combustivel = carro.Combustivel,
                ImagemUrl = carro.ImagemUrl,
                Descricao = carro.Descricao,
                Marca = carro.Marca,
                Categoria = carro.Categoria
            };

            return View(detalhes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter detalhes do carro id={CarroId}", id);
            return StatusCode(500);
        }
    }
}
