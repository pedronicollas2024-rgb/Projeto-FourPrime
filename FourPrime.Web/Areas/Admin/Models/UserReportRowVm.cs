namespace FourPrime.Web.Areas.Admin.Models;

public class UserReportRowVm
{
    public string UserId { get; set; } = "";
    public string? NomeCompleto { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string Role { get; set; } = "Sem role";
}