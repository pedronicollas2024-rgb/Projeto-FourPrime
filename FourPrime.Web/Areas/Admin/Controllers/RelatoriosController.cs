using FourPrime.Infrastructure.Persistence;
using FourPrime.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FourPrime.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class RelatoriosController : Controller
{
    private readonly AppDbContext _context;

    public RelatoriosController(AppDbContext context)
    {
        _context = context;
    }

    private bool CheckAdminAccess()
    {
        if (HttpContext.Session.GetString("UsuarioLogado") != "true")
            return false;

        var role = HttpContext.Session.GetString("UsuarioRole");
        return role == "Admin" || role == "Manager";
    }

    public async Task<IActionResult> Index()
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Login", "Auth", new { area = "" });

        // ⚠️ Isso assume que seu AppDbContext herda de IdentityDbContext (tem Users/Roles/UserRoles).
        // Se não tiver, me manda o AppDbContext que eu adapto em 2 minutos.
        var users = await _context.Users
            .AsNoTracking()
            .Select(u => new { u.Id, u.UserName, u.Email, NomeCompleto = EF.Property<string>(u, "NomeCompleto") })
            .ToListAsync();

        var roles = await _context.Roles.AsNoTracking().ToListAsync();
        var userRoles = await _context.UserRoles.AsNoTracking().ToListAsync();

        var rows = users.Select(u =>
        {
            var roleId = userRoles.FirstOrDefault(ur => ur.UserId == u.Id)?.RoleId;
            var roleName = roles.FirstOrDefault(r => r.Id == roleId)?.Name ?? "Sem role";

            return new UserReportRowVm
            {
                UserId = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                NomeCompleto = u.NomeCompleto,
                Role = roleName
            };
        })
        .OrderBy(r => r.Role)
        .ThenBy(r => r.Email)
        .ToList();

        return View(rows);
    }
}