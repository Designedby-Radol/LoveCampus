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
        "Server=localhost;Database=campus_love;User=campus2023;Password=campus2023;Port=3306;";
}