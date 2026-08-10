using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FourPrime.Infrastructure;
using FourPrime.Infrastructure.Database;
using FourPrime.Infrastructure.Persistence;
using FourPrime.Web.Areas.Admin.Services;
using FourPrime.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// =======================
// SERVICES
// =======================

builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();

// Infrastructure (FourPrime)
builder.Services.AddInfrastructure(builder.Configuration);

// DbContext do FourPrime (mesmo banco)
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// =======================
// AUTHENTICATION (GOOGLE)
// =======================

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Google";
})
.AddCookie("Cookies")
.AddGoogle("Google", options =>
{
    options.ClientId = builder.Configuration["GoogleAuth:ClientId"] ?? "";
    options.ClientSecret = builder.Configuration["GoogleAuth:ClientSecret"] ?? "";
    options.CallbackPath = "/signin-google";
});


// =======================
// HttpClient (API)
// =======================

var apiUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5138";

builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri(apiUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// =======================
// Custom services
// =======================

builder.Services.AddScoped<DashboardService>();

var app = builder.Build();

// =======================
// PIPELINE
// =======================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// NÃO USAR HTTPS
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // precisa vir antes do endpoint usar Session

// ⚠️ AUTH vem antes do Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/desktop-login-identity", (string token, HttpContext http, IConfiguration config) =>
{
    if (string.IsNullOrWhiteSpace(token))
        return Results.BadRequest("Token inválido.");

    var key = config["Jwt:Key"];
    var issuer = config["Jwt:Issuer"];
    var audience = config["Jwt:Audience"];

    if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
        return Results.Problem("JWT não configurado no Web (appsettings).");

    var tokenHandler = new JwtSecurityTokenHandler();
    try
    {
        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        }, out _);

        var email = principal.FindFirstValue(ClaimTypes.Email) ?? "";
        var nome = principal.FindFirstValue(ClaimTypes.Name) ?? "";
        var role = principal.FindFirstValue(ClaimTypes.Role) ?? "User";

        // Session igual seu DashboardController espera
        http.Session.SetString("UsuarioLogado", "true");
        http.Session.SetString("UsuarioEmail", email);
        http.Session.SetString("UsuarioNome", nome);
        http.Session.SetString("UsuarioRole", role);

        if (role == "Admin" || role == "Manager")
            return Results.Redirect("/Admin/Dashboard/Index");

        return Results.Redirect("/");
    }
    catch
    {
        return Results.Unauthorized();
    }
});

// =======================
// DESKTOP LOGIN ENDPOINT
// =======================

app.MapGet("/desktop-login", async (string token, AppDbContext db, HttpContext http) =>
{
    if (string.IsNullOrWhiteSpace(token))
        return Results.BadRequest("Token inválido.");

    var sessao = await db.Sessoes.FirstOrDefaultAsync(s =>
        s.TokenSessao == token &&
        s.Ativa &&
        s.ExpiraEm > DateTime.UtcNow);

    if (sessao is null)
        return Results.Unauthorized();

    var usuario = await db.Usuarios.FirstOrDefaultAsync(u =>
        u.Id == sessao.UsuarioId &&
        u.Ativo);

    if (usuario is null)
        return Results.Unauthorized();

    var role = usuario.Perfil ?? "";

    if (!role.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
        !role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
        return Results.Forbid();

    // Session do site (compatível com seu DashboardController)
    http.Session.SetString("UsuarioLogado", "true");
    http.Session.SetString("UsuarioRole", role);
    http.Session.SetString("UsuarioId", usuario.Id.ToString());
    http.Session.SetString("UsuarioNome", usuario.NomeDeUsuario);

    // opcional: token uso único
    sessao.Ativa = false;
    sessao.EncerradaEm = DateTime.UtcNow;
    await db.SaveChangesAsync();

    return Results.Redirect("/Admin/Dashboard/Index");
});

// =======================
// ROUTES
// =======================

app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Console.WriteLine("🚀 WEB em http://localhost:5043");
Console.WriteLine($"🔗 API: {apiUrl}");

app.Run();