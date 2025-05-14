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
        "Server=localhost;Database=campus_love;User=root;Password=11358;Port=5506;";
} 