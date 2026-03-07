using System.ComponentModel.DataAnnotations;

namespace FourPrime.Api.Models;

public class RegisterDto
{
    [Required]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
