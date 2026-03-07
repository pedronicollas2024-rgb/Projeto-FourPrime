using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FourPrime.Web.Areas.Admin.Models;
using FourPrime.Infrastructure.Persistence;
using FourPrime.Domain.Entities;

namespace FourPrime.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class MarcasController : Controller
{
    private readonly AppDbContext _context;

    public MarcasController(AppDbContext context)
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

    // GET: Admin/Marcas
    public async Task<IActionResult> Index()
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Login", "Auth", new { area = "", returnUrl = "/Admin/Marcas" });

        var marcas = await _context.Marcas
            .Include(m => m.Carros)
            .OrderBy(m => m.Nome)
            .ToListAsync();

        return View(marcas);
    }



    // GET: Admin/Marcas/Create
    public IActionResult Create()
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Login", "Auth", new { area = "", returnUrl = "/Admin/Marcas/Create" });

        return View(new MarcaEditVm());
    }

    // POST: Admin/Marcas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MarcaEditVm model)
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Login", "Auth", new { area = "" });

        if (ModelState.IsValid)
        {
            var marca = new Marca
            {
                Nome = model.Nome,
                PaisOrigem = model.PaisOrigem,
                AnoFundacao = model.AnoFundacao
            };

            _context.Add(marca);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Marca criada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Admin/Marcas/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Login", "Auth", new { area = "", returnUrl = $"/Admin/Marcas/Edit/{id}" });

        if (id == null) return NotFound();

        var marca = await _context.Marcas.FindAsync(id);
        if (marca == null) return NotFound();

        var model = new MarcaEditVm
        {
            Id = marca.Id,
            Nome = marca.Nome,
            PaisOrigem = marca.PaisOrigem,
            AnoFundacao = marca.AnoFundacao
        };

        return View(model);
    }

    // POST: Admin/Marcas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MarcaEditVm model)
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Login", "Auth", new { area = "" });

        if (id != model.Id) return NotFound();

        if (ModelState.IsValid)
        {
            var marca = await _context.Marcas.FindAsync(id);
            if (marca == null) return NotFound();

            marca.Nome = model.Nome;
            marca.PaisOrigem = model.PaisOrigem;
            marca.AnoFundacao = model.AnoFundacao;

            try
            {
                _context.Update(marca);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Marca atualizada com sucesso!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarcaExists(id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Admin/Marcas/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        // ✅ APENAS ADMIN
        if (!IsAdmin())
            return RedirectToAction("AccessDenied", "Auth", new { area = "" });

        if (id == null) return NotFound();

        var marca = await _context.Marcas
            .Include(m => m.Carros)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (marca == null) return NotFound();

        return View(marca);
    }

    // POST: Admin/Marcas/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // ✅ APENAS ADMIN
        if (!IsAdmin())
            return RedirectToAction("AccessDenied", "Auth", new { area = "" });

        var marca = await _context.Marcas
            .Include(m => m.Carros)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (marca == null) return NotFound();

        // 🔥 VALIDAÇÃO INTELIGENTE - Não permite excluir se houver carros
        if (marca.Carros.Any())
        {
            TempData["Error"] = $"Não é possível excluir a marca '{marca.Nome}'. Existem {marca.Carros.Count} carro(s) vinculados a ela.";
            return RedirectToAction(nameof(Index));
        }

        _context.Marcas.Remove(marca);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Marca excluída com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    private bool MarcaExists(int id)
    {
        return _context.Marcas.Any(e => e.Id == id);
    }
}