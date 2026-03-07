using System.Text.Json;
using FourPrime.Web.Models;

namespace FourPrime.Web.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<CarroDto>> GetCarrosAsync(CatalogFilterModel filter)
    {
        try
        {
            var queryString = BuildQueryString(filter);
            var response = await _httpClient.GetAsync($"api/carros{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CarroDto>>(content, _jsonOptions) ?? new List<CarroDto>();
            }

            Console.WriteLine($"❌ API Error: {response.StatusCode} - {response.ReasonPhrase}");
            return new List<CarroDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ API Exception: {ex.Message}");
            return new List<CarroDto>();
        }
    }

    public async Task<CarroDto?> GetCarroByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/carros/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<CarroDto>(content, _jsonOptions);
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ API Exception: {ex.Message}");
            return null;
        }
    }

    public async Task<List<MarcaDto>> GetMarcasAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/marcas");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<MarcaDto>>(content, _jsonOptions) ?? new List<MarcaDto>();
            }

            return new List<MarcaDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ API Exception: {ex.Message}");
            return new List<MarcaDto>();
        }
    }

    public async Task<List<CategoriaDto>> GetCategoriasAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/categorias");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CategoriaDto>>(content, _jsonOptions) ?? new List<CategoriaDto>();
            }

            return new List<CategoriaDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ API Exception: {ex.Message}");
            return new List<CategoriaDto>();
        }
    }

    private static string BuildQueryString(CatalogFilterModel filter)
    {
        var queryParams = new List<string>();

        // ✅ AGORA MANDA O TERMO CERTO PRA API (para buscar em Modelo/Cor/Categoria/Marca etc.)
        if (!string.IsNullOrWhiteSpace(filter.TermoPesquisa))
            queryParams.Add($"termoPesquisa={Uri.EscapeDataString(filter.TermoPesquisa)}");

        // ✅ Mantém filtro específico de Modelo (se você quiser usar separado)
        if (!string.IsNullOrWhiteSpace(filter.Modelo))
            queryParams.Add($"modelo={Uri.EscapeDataString(filter.Modelo)}");

        if (filter.MarcaId.HasValue)
            queryParams.Add($"marcaId={filter.MarcaId.Value}");

        if (filter.CategoriaId.HasValue)
            queryParams.Add($"categoriaId={filter.CategoriaId.Value}");

        if (filter.AnoMin.HasValue)
            queryParams.Add($"anoMin={filter.AnoMin.Value}");

        if (filter.AnoMax.HasValue)
            queryParams.Add($"anoMax={filter.AnoMax.Value}");

        if (filter.PrecoMin.HasValue)
            queryParams.Add($"precoMin={filter.PrecoMin.Value}");

        if (filter.PrecoMax.HasValue)
            queryParams.Add($"precoMax={filter.PrecoMax.Value}");

        if (!string.IsNullOrWhiteSpace(filter.SortBy))
            queryParams.Add($"sortBy={Uri.EscapeDataString(filter.SortBy)}");

        if (filter.Desc)
            queryParams.Add("desc=true");

        return queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
    }
    public async Task<List<CarroDto>> GetDestaquesAsync(int take = 6)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/carros/destaques?take={take}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CarroDto>>(content, _jsonOptions) ?? new List<CarroDto>();
            }

            Console.WriteLine($"❌ API Error: {response.StatusCode} - {response.ReasonPhrase}");
            return new List<CarroDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ API Exception: {ex.Message}");
            return new List<CarroDto>();
        }
    }

    public async Task<List<CarroDto>> GetDestaquesLojaAsync(string tipo, int take = 8)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/carros/destaques-loja?tipo={Uri.EscapeDataString(tipo)}&take={take}");
            if (!response.IsSuccessStatusCode) return new();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CarroDto>>(content, _jsonOptions) ?? new();
        }
        catch
        {
            return new();
        }
    }



}
