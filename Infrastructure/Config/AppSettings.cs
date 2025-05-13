using Microsoft.Extensions.Configuration;

namespace LoveCampus.Infrastructure.Config;

public static class AppSettings
{
    private static IConfiguration? _configuration;

    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static string ConnectionString => 
        _configuration?.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("La cadena de conexión no está configurada.");

    public static string GetValue(string key)
    {
        return _configuration?[key] 
            ?? throw new InvalidOperationException($"La configuración '{key}' no está definida.");
    }
} 