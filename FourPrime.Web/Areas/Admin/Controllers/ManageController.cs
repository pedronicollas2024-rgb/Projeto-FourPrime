using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FourPrime.Application.Services;
using FourPrime.Application.DTOs;
using FourPrime.Application.Filters;
using FourPrime.Domain.Entities;

namespace FourPrime.Web.Areas.Admin.Controllers;

[Area("Admin")]
// ❌ REMOVIDO: [Authorize(Roles = "Admin,Manager")]
public class ManageController : Controller
{
    private readonly ICarroQueryService _carroQueryService;
    private readonly ICatalogLookupService _catalogLookupService;

    public ManageController(
        ICarroQueryService carroQueryService,
        ICatalogLookupService catalogLookupService)
    {
        _carroQueryService = carroQueryService;
        _catalogLookupService = catalogLookupService;
    }

    private bool CheckAdminAccess()
    {
        if (HttpContext.Session.GetString("UsuarioLogado") != "true")
        {
            return false;
        }

        var userRole = HttpContext.Session.GetString("UsuarioRole");
        return userRole == "Admin" || userRole == "Manager";
    }

    public async Task<IActionResult> Index()
    {
        // ✅ VERIFICAÇÃO MANUAL
        if (!CheckAdminAccess())
        {
            return RedirectToAction("Login", "Auth", new { area = "", returnUrl = "/Admin/Manage" });
        }

        // Carregar estatísticas para o dashboard
        ViewData["TotalCarros"] = await _carroQueryService.GetTotalCarrosAsync();

        var marcas = await _catalogLookupService.GetMarcasAsync();
        ViewData["TotalMarcas"] = marcas.Count;

        var categorias = await _catalogLookupService.GetCategoriasAsync();
        ViewData["TotalCategorias"] = categorias.Count;

        ViewData["TotalUsuarios"] = 3; // Admin, Manager, User

        return View();
    }

    public async Task<IActionResult> Carros()
    {
        if (!CheckAdminAccess()) return RedirectToAction("Login", "Auth", new { area = "" });

        ViewData["Title"] = "Gerenciar Carros";
        ViewData["Tipo"] = "Carros";
        var totalCarros = await _carroQueryService.GetTotalCarrosAsync();
        ViewData["TotalCarros"] = totalCarros;
        return View();
    }

    public async Task<IActionResult> Marcas()
    {
        if (!CheckAdminAccess()) return RedirectToAction("Login", "Auth", new { area = "" });

        ViewData["Title"] = "Gerenciar Marcas";
        ViewData["Tipo"] = "Marcas";
        var marcas = await _catalogLookupService.GetMarcasAsync();
        ViewData["TotalMarcas"] = marcas.Count;
        return View();
    }

    public async Task<IActionResult> Categorias()
    {
        if (!CheckAdminAccess()) return RedirectToAction("Login", "Auth", new { area = "" });

        ViewData["Title"] = "Gerenciar Categorias";
        ViewData["Tipo"] = "Categorias";
        var categorias = await _catalogLookupService.GetCategoriasAsync();
        ViewData["TotalCategorias"] = categorias.Count;
        return View();
    }

    public IActionResult Usuarios()
    {
        // ✅ APENAS ADMIN
        if (HttpContext.Session.GetString("UsuarioLogado") != "true" ||
            HttpContext.Session.GetString("UsuarioRole") != "Admin")
        {
            return RedirectToAction("AccessDenied", "Auth", new { area = "" });
        }

        ViewData["Title"] = "Gerenciar Usuários";
        ViewData["Tipo"] = "Usuários";
        ViewData["TotalUsuarios"] = 3;
        return View();
    }

    // ... resto dos métodos mantidos (remover [Authorize] deles também)
}