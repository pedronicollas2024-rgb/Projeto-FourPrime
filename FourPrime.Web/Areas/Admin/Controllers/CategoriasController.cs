using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FourPrime.Web.Areas.Admin.Models;
using FourPrime.Infrastructure.Persistence;
using FourPrime.Domain.Entities;

namespace FourPrime.Web.Areas.Admin.Controllers;

[Area("Admin")]
// ❌ REMOVIDO: [Authorize(Roles = "Admin,Manager")]
public class CategoriasController : Controller
{
    private readonly AppDbContext _context;

    public CategoriasController(AppDbContext context)
    {
        _context = context;
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

    private bool IsAdmin()
    {
        return HttpContext.Session.GetString("UsuarioRole") == "Admin";
    }

    // GET: Admin/Categorias
    public async Task<IActionResult> Index()
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Login", "Auth", new { area = "", returnUrl = "/Admin/Categorias" });

        var categorias = await _context.Categorias.OrderBy(c => c.Nome).ToListAsync();
        return View(categorias);
    }

    // GET: Admin/Categorias/Create
    public IActionResult Create()
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Login", "Auth", new { area = "", returnUrl = "/Admin/Categorias/Create" });

        return View();
    }

    // POST: Admin/Categorias/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoriaEditVm model)
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Login", "Auth", new { area = "" });

        if (ModelState.IsValid)
        {
            var categoria = new Categoria
            {
                Nome = model.Nome,
                Descricao = model.Descricao
            };

            _context.Add(categoria);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Categoria criada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Admin/Categorias/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Login", "Auth", new { area = "", returnUrl = "/Admin/Categorias/Edit" });

        if (id == null) return NotFound();

        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null) return NotFound();

        var model = new CategoriaEditVm
        {
            Id = categoria.Id,
            Nome = categoria.Nome,
            Descricao = categoria.Descricao
        };

        return View(model);
    }

    // POST: Admin/Categorias/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoriaEditVm model)
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Login", "Auth", new { area = "" });

        if (id != model.Id) return NotFound();

        if (ModelState.IsValid)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();

            categoria.Nome = model.Nome;
            categoria.Descricao = model.Descricao;

            try
            {
                _context.Update(categoria);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Categoria atualizada com sucesso!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaExists(id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Admin/Categorias/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        // ✅ APENAS ADMIN
        if (!IsAdmin())
            return RedirectToAction("AccessDenied", "Auth", new { area = "" });

        if (id == null) return NotFound();

        var categoria = await _context.Categorias
            .Include(c => c.Carros)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria == null) return NotFound();

        // Verificar se há carros associados
        if (categoria.Carros.Any())
        {
            TempData["Error"] = "Não é possível excluir esta categoria pois existem carros associados a ela.";
            return RedirectToAction(nameof(Index));
        }

        return View(categoria);
    }

    // POST: Admin/Categorias/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // ✅ APENAS ADMIN
        if (!IsAdmin())
            return RedirectToAction("AccessDenied", "Auth", new { area = "" });

        var categoria = await _context.Categorias
            .Include(c => c.Carros)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria == null) return NotFound();

        if (categoria.Carros.Any())
        {
            TempData["Error"] = "Não é possível excluir esta categoria pois existem carros associados a ela.";
            return RedirectToAction(nameof(Index));
        }

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Categoria excluída com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    private bool CategoriaExists(int id)
    {
        return _context.Categorias.Any(e => e.Id == id);
    }
}