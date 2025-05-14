namespace LoveCampus.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Edad { get; set; }
    public int GeneroId { get; set; }
    public int CarreraId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FrasePerfil { get; set; }
    public int CreditosDisponibles { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime? UltimoAcceso { get; set; }

    // Propiedades de navegación
    public string? GeneroDescripcion { get; set; }
    public string? CarreraNombre { get; set; }
    public List<string>? Intereses { get; set; }
} 