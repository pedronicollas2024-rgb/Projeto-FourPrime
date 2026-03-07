using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FourPrime.Infrastructure.Entities;
using FourPrime.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace FourPrime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // ============================
    // REGISTER
    // ============================
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            NomeCompleto = model.NomeCompleto,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Usuário padrão
        await _userManager.AddToRoleAsync(user, "User");

        return Ok("Usuário criado com sucesso");
    }

    // ============================
    // LOGIN
    // ============================
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            false,
            false);

        if (!result.Succeeded)
            return Unauthorized("Login inválido");

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return Unauthorized("Login inválido");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";

        // JWT
        var key = Encoding.UTF8.GetBytes(HttpContext.RequestServices
            .GetRequiredService<IConfiguration>()["Jwt:Key"]!);

        var issuer = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Issuer"]!;
        var audience = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Audience"]!;
        var expiresMinutes = int.Parse(HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:ExpiresMinutes"] ?? "60");

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email ?? ""),
        new Claim(ClaimTypes.Name, user.NomeCompleto ?? ""),
        new Claim(ClaimTypes.Role, role)
    };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiresMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);

        return Ok(new
        {
            user.Email,
            user.NomeCompleto,
            Role = role,
            Token = token
        });
    }

    // ============================
    // LOGOUT
    // ============================
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }
}
