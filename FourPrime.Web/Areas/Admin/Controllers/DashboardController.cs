using Microsoft.AspNetCore.Mvc;
using FourPrime.Web.Areas.Admin.Services;

namespace FourPrime.Web.Areas.Admin.Controllers;

[Area("Admin")]
// ❌ REMOVIDO: [Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        // ✅ VERIFICAÇÃO MANUAL COM SESSION
        if (HttpContext.Session.GetString("UsuarioLogado") != "true")
        {
            return RedirectToAction("Login", "Auth", new { area = "", returnUrl = "/Admin/Dashboard" });
        }

        var userRole = HttpContext.Session.GetString("UsuarioRole");
        if (userRole != "Admin" && userRole != "Manager")
        {
            return RedirectToAction("AccessDenied", "Auth", new { area = "" });
        }

        var data = await _dashboardService.GetDashboardDataAsync();
        return View(data);
    }
}