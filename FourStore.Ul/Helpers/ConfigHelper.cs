using Microsoft.Extensions.Configuration;

namespace FourPrime.UI;

public static class ConfigHelper
{
    public static string GetDefaultConnection()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        return config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection não encontrada.");
    }
}