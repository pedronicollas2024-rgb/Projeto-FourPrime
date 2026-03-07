namespace FourPrime.Web.Views.Shared;

public static class CacheKeys
{
    public const string Marcas = "MarcasCache";
    public const string Categorias = "CategoriasCache";
    public const string CarrosRecentes = "CarrosRecentesCache";
    public const string TotalCarros = "TotalCarrosCache";
    public static TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
}