//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using FourPrime.Web.Areas.Admin.Models;
//using FourPrime.Infrastructure.Entities;
//using FourPrime.Infrastructure.Persistence;

//namespace FourPrime.Web.Areas.Admin.Controllers;

//[Area("Admin")]
//[Authorize(Roles = "Admin")]
//public class UsuariosController : Controller
//{
//    private readonly UserManager<ApplicationUser> _userManager;
//    private readonly RoleManager<IdentityRole> _roleManager;

//    public UsuariosController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
//    {
//        _userManager = userManager;
//        _roleManager = roleManager;
//    }

//    // GET: Admin/Usuarios
//    public async Task<IActionResult> Index()
//    {
//        var users = await _userManager.Users
//            .OrderBy(u => u.NomeCompleto)
//            .ToListAsync();

//        var userVms = new List<UsuarioEditVm>();
//        foreach (var user in users)
//        {
//            var roles = await _userManager.GetRolesAsync(user);
//            userVms.Add(new UsuarioEditVm
//            {
//                Id = user.Id,
//                NomeCompleto = user.NomeCompleto ?? string.Empty,
//                Email = user.Email ?? string.Empty,
//                Ativo = user.Ativo,
//                Role = roles.FirstOrDefault() ?? "User"
//            });
//        }

//        return View(userVms);
//    }

//    // GET: Admin/Usuarios/Create
//    public async Task<IActionResult> Create()
//    {
//        var model = new UsuarioEditVm
//        {
//            Roles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync()
//        };
//        return View(model);
//    }

//    // POST: Admin/Usuarios/Create
//    [HttpPost]
//    [ValidateAntiForgeryToken]
//    public async Task<IActionResult> Create(UsuarioEditVm model)
//    {
//        if (ModelState.IsValid)
//        {
//            var user = new ApplicationUser
//            {
//                UserName = model.Email,
//                Email = model.Email,
//                NomeCompleto = model.NomeCompleto,
//                Ativo = model.Ativo,
//                DataCriacao = DateTime.UtcNow
//            };

//            // Senha padrão - usuário pode alterar depois
//            var result = await _userManager.CreateAsync(user, "Senha@123");

//            if (result.Succeeded)
//            {
//                if (!string.IsNullOrEmpty(model.Role))
//                {
//                    await _userManager.AddToRoleAsync(user, model.Role);
//                }
//                TempData["Success"] = "Usuário criado com sucesso! A senha padrão é: Senha@123";
//                return RedirectToAction(nameof(Index));
//            }

//            foreach (var error in result.Errors)
//            {
//                ModelState.AddModelError(string.Empty, error.Description);
//            }
//        }

//        model.Roles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
//        return View(model);
//    }

//    // GET: Admin/Usuarios/Edit/5
//    public async Task<IActionResult> Edit(string id)
//    {
//        if (string.IsNullOrEmpty(id)) return NotFound();

//        var user = await _userManager.FindByIdAsync(id);
//        if (user == null) return NotFound();

//        var roles = await _userManager.GetRolesAsync(user);

//        var model = new UsuarioEditVm
//        {
//            Id = user.Id,
//            NomeCompleto = user.NomeCompleto ?? string.Empty,
//            Email = user.Email ?? string.Empty,
//            Ativo = user.Ativo,
//            Role = roles.FirstOrDefault() ?? "User",
//            Roles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync()
//        };

//        return View(model);
//    }

//    // POST: Admin/Usuarios/Edit/5
//    [HttpPost]
//    [ValidateAntiForgeryToken]
//    public async Task<IActionResult> Edit(string id, UsuarioEditVm model)
//    {
//        if (id != model.Id) return NotFound();

//        if (ModelState.IsValid)
//        {
//            var user = await _userManager.FindByIdAsync(id);
//            if (user == null) return NotFound();

//            user.NomeCompleto = model.NomeCompleto;
//            user.Email = model.Email;
//            user.UserName = model.Email;
//            user.Ativo = model.Ativo;

//            var result = await _userManager.UpdateAsync(user);
//            if (result.Succeeded)
//            {
//                // Atualizar role
//                var currentRoles = await _userManager.GetRolesAsync(user);
//                await _userManager.RemoveFromRolesAsync(user, currentRoles);

//                if (!string.IsNullOrEmpty(model.Role))
//                {
//                    await _userManager.AddToRoleAsync(user, model.Role);
//                }

//                TempData["Success"] = "Usuário atualizado com sucesso!";
//                return RedirectToAction(nameof(Index));
//            }

//            foreach (var error in result.Errors)
//            {
//                ModelState.AddModelError(string.Empty, error.Description);
//            }
//        }

//        model.Roles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
//        return View(model);
//    }

//    // POST: Admin/Usuarios/ToggleStatus/5
//    [HttpPost]
//    [ValidateAntiForgeryToken]
//    public async Task<IActionResult> ToggleStatus(string id)
//    {
//        var user = await _userManager.FindByIdAsync(id);
//        if (user != null)
//        {
//            user.Ativo = !user.Ativo;
//            await _userManager.UpdateAsync(user);
//            TempData["Success"] = $"Usuário {(user.Ativo ? "ativado" : "desativado")} com sucesso!";
//        }
//        return RedirectToAction(nameof(Index));
//    }
//}