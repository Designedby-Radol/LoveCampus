namespace LoveCampus.Domain.Entities;

public class ProductoTienda
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int PrecioCapcoins { get; set; }
    public string Tipo { get; set; } = string.Empty; // "token" o "like"
    public int Cantidad { get; set; }
} 