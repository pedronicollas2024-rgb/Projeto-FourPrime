using FourPrime.Domain.Entities;
using FourPrime.Infrastructure.Persistence;
using FourPrime.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FourPrime.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class CarrosController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public CarrosController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    private bool CheckAdminAccess()
    {
        if (HttpContext.Session.GetString("UsuarioLogado") != "true")
            return false;

        var role = HttpContext.Session.GetString("UsuarioRole");
        return role == "Admin" || role == "Manager";
    }

    private bool IsAdmin()
    {
        return HttpContext.Session.GetString("UsuarioRole") == "Admin";
    }

    private static readonly string[] DestaqueTipos = new[] { "Blindados", "Esportivos", "Luxos" };

    // ============================================================
    // GET: Admin/Carros
    // ============================================================
    public async Task<IActionResult> Index()
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Login", "Auth", new { area = "" });

        var carros = await _context.Carros
            .Include(c => c.Marca)
            .Include(c => c.Categoria)
            .OrderBy(c => c.Modelo)
            .ToListAsync();

        return View(carros);
    }

    // ============================================================
    // GET: Admin/Carros/Details/5
    // ============================================================
    public async Task<IActionResult> Details(int? id)
    {
        if (!CheckAdminAccess()) return RedirectToAction("Login", "Auth", new { area = "" });
        if (id == null) return NotFound();

        var carro = await _context.Carros
            .Include(c => c.Marca)
            .Include(c => c.Categoria)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (carro == null) return NotFound();
        return View(carro);
    }

    // ============================================================
    // GET: Admin/Carros/Create
    // ============================================================
    public async Task<IActionResult> Create()
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Login", "Auth", new { area = "" });

        ViewBag.DestaqueTipos = DestaqueTipos;

        var vm = new CarroEditVm
        {
            Marcas = await _context.Marcas.OrderBy(m => m.Nome).ToListAsync(),
            Categorias = await _context.Categorias.OrderBy(c => c.Nome).ToListAsync()
        };

        return View(vm);
    }

    // ============================================================
    // POST: Admin/Carros/Create
    // ============================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CarroEditVm model)
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Login", "Auth", new { area = "" });

        ViewBag.DestaqueTipos = DestaqueTipos;

        if (!ModelState.IsValid)
        {
            model.Marcas = await _context.Marcas.OrderBy(m => m.Nome).ToListAsync();
            model.Categorias = await _context.Categorias.OrderBy(c => c.Nome).ToListAsync();
            return View(model);
        }

        var carro = new Carro
        {
            Modelo = model.Modelo,
            Ano = model.Ano,
            Preco = model.Preco,
            MarcaId = model.MarcaId,
            CategoriaId = model.CategoriaId,
            Cor = model.Cor,
            Quilometragem = model.Quilometragem,
            Descricao = model.Descricao,
            Combustivel = model.Combustivel,
            IsDestaque = model.IsDestaque,
            DestaqueTipo = model.DestaqueTipo
        };

        // se marcou destaque e não escolheu tipo, define padrão
        if (carro.IsDestaque && string.IsNullOrWhiteSpace(carro.DestaqueTipo))
            carro.DestaqueTipo = "Esportivos";

        // se NÃO marcou destaque, limpa o tipo
        if (!carro.IsDestaque)
            carro.DestaqueTipo = null;

        if (model.ImagemFile != null && model.ImagemFile.Length > 0)
        {
            var folder = Path.Combine(_environment.WebRootPath, "uploads", "carros");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid() + Path.GetExtension(model.ImagemFile.FileName);
            string filePath = Path.Combine(folder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await model.ImagemFile.CopyToAsync(stream);

            carro.ImagemUrl = "/uploads/carros/" + fileName;
        }

        _context.Carros.Add(carro);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Carro criado com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    // ============================================================
    // GET: Admin/Carros/Edit/5
    // ============================================================
    public async Task<IActionResult> Edit(int? id)
    {
        if (!CheckAdminAccess()) return RedirectToAction("Login", "Auth", new { area = "" });
        if (id == null) return NotFound();

        ViewBag.DestaqueTipos = DestaqueTipos;

        var carro = await _context.Carros.FindAsync(id);
        if (carro == null) return NotFound();

        var vm = new CarroEditVm
        {
            Id = carro.Id,
            Modelo = carro.Modelo,
            Ano = carro.Ano,
            Preco = carro.Preco,
            MarcaId = carro.MarcaId,
            CategoriaId = carro.CategoriaId,
            Cor = carro.Cor,
            Quilometragem = carro.Quilometragem,
            Descricao = carro.Descricao,
            ImagemUrl = carro.ImagemUrl,
            Combustivel = carro.Combustivel,
            IsDestaque = carro.IsDestaque,
            DestaqueTipo = carro.DestaqueTipo,
            Marcas = await _context.Marcas.OrderBy(m => m.Nome).ToListAsync(),
            Categorias = await _context.Categorias.OrderBy(c => c.Nome).ToListAsync()
        };

        return View(vm);
    }

    // ============================================================
    // POST: Admin/Carros/Edit/5
    // ============================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CarroEditVm model)
    {
        if (!CheckAdminAccess()) return RedirectToAction("Login", "Auth", new { area = "" });
        if (id != model.Id) return NotFound();

        ViewBag.DestaqueTipos = DestaqueTipos;

        if (!ModelState.IsValid)
        {
            model.Marcas = await _context.Marcas.OrderBy(m => m.Nome).ToListAsync();
            model.Categorias = await _context.Categorias.OrderBy(c => c.Nome).ToListAsync();
            return View(model);
        }

        var carro = await _context.Carros.FindAsync(id);
        if (carro == null) return NotFound();

        carro.Modelo = model.Modelo;
        carro.Ano = model.Ano;
        carro.Preco = model.Preco;
        carro.MarcaId = model.MarcaId;
        carro.CategoriaId = model.CategoriaId;
        carro.Cor = model.Cor;
        carro.Descricao = model.Descricao;
        carro.Quilometragem = model.Quilometragem;
        carro.Combustivel = model.Combustivel;

        carro.IsDestaque = model.IsDestaque;
        carro.DestaqueTipo = model.DestaqueTipo;

        // se marcou destaque e não escolheu tipo, define padrão
        if (carro.IsDestaque && string.IsNullOrWhiteSpace(carro.DestaqueTipo))
            carro.DestaqueTipo = "Esportivos";

        // se NÃO marcou destaque, limpa o tipo
        if (!carro.IsDestaque)
            carro.DestaqueTipo = null;

        if (model.ImagemFile != null && model.ImagemFile.Length > 0)
        {
            var folder = Path.Combine(_environment.WebRootPath, "uploads", "carros");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid() + Path.GetExtension(model.ImagemFile.FileName);
            string filePath = Path.Combine(folder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await model.ImagemFile.CopyToAsync(stream);

            carro.ImagemUrl = "/uploads/carros/" + fileName;
        }

        _context.Update(carro);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Carro atualizado com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    // ============================================================
    // DELETE
    // ============================================================
    public async Task<IActionResult> Delete(int? id)
    {
        if (!IsAdmin()) return RedirectToAction("AccessDenied", "Auth", new { area = "" });
        if (id == null) return NotFound();

        var carro = await _context.Carros
            .Include(c => c.Marca)
            .Include(c => c.Categoria)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (carro == null) return NotFound();
        return View(carro);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!IsAdmin()) return RedirectToAction("AccessDenied", "Auth", new { area = "" });

        var carro = await _context.Carros.FindAsync(id);
        if (carro == null) return NotFound();

        _context.Carros.Remove(carro);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Carro excluído com sucesso!";
        return RedirectToAction(nameof(Index));
    }
}
