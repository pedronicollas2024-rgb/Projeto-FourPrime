using Microsoft.AspNetCore.Mvc;
using FourPrime.Web.Models;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;


namespace FourPrime.Web.Controllers;

public class AuthController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    public AuthController(
        IHttpClientFactory httpClientFactory,
        ILogger<AuthController> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
    }

    // =========================
    // LOGIN
    // =========================
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVm model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = _httpClientFactory.CreateClient();

        var apiUrl = _configuration["ApiBaseUrl"];

        var payload = new
        {
            email = model.Email,
            password = model.Password
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync(
            $"{apiUrl}/api/Auth/login",
            content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Login inválido");
            return View(model);
        }

        var json = await response.Content.ReadAsStringAsync();

        var user = JsonSerializer.Deserialize<LoginResponse>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Salva na Session
        HttpContext.Session.SetString("UsuarioLogado", "true");
        HttpContext.Session.SetString("UsuarioEmail", user!.Email);
        HttpContext.Session.SetString("UsuarioRole", user.Role);

        _logger.LogInformation("Login: {Email}", user.Email);

        // Redireciona conforme perfil
        if (user.Role == "Admin" || user.Role == "Manager")
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        return RedirectToAction("Index", "Home");

    }

    // =========================
    // REGISTER
    // =========================

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVm model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = _httpClientFactory.CreateClient();

        var apiUrl = _configuration["ApiBaseUrl"];

        var payload = new
        {
            nomeCompleto = model.NomeCompleto,
            email = model.Email,
            password = model.Password
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync(
            $"{apiUrl}/api/Auth/register",
            content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Erro ao cadastrar usuário");
            return View(model);
        }

        TempData["Sucesso"] = "Conta criada com sucesso! Faça login.";

        return RedirectToAction("Login");
    }

    // =========================
    // LOGOUT
    // =========================

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    // =========================
    // GOOGLE LOGIN
    // =========================

    [HttpGet]
    public IActionResult GoogleLogin(string? returnUrl = null)
    {
        var redirectUrl = Url.Action("GoogleCallback", "Auth", new { returnUrl });
        var props = new AuthenticationProperties { RedirectUri = redirectUrl };

        return Challenge(props, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public async Task<IActionResult> GoogleCallback(string? returnUrl = null)
    {
        // pega o usuário autenticado pelo Google (cookie temporário do provider)
        var result = await HttpContext.AuthenticateAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded || result.Principal == null)
            return RedirectToAction("Login");

        var email = result.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
        var nome = result.Principal.FindFirstValue(ClaimTypes.Name) ?? "";

        // ✅ aqui por enquanto vamos manter o padrão:
        // - se entrar por Google, vira "User"
        // - grava na Session igual seu login normal
        HttpContext.Session.SetString("UsuarioLogado", "true");
        HttpContext.Session.SetString("UsuarioEmail", email);
        HttpContext.Session.SetString("UsuarioRole", "User");

        // opcional: guardar nome também
        HttpContext.Session.SetString("UsuarioNome", nome);

        // limpa o cookie de autenticação do provider pra não ficar “logado” no esquema de cookie
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

}


// =========================
// DTO LOGIN RESPONSE
// =========================

public class LoginResponse
{
    public string Email { get; set; } = "";
    public string NomeCompleto { get; set; } = "";
    public string Role { get; set; } = "";
}
