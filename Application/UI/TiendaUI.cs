using LoveCampus.Infrastructure.Repositories;
using LoveCampus.Domain.Entities;

namespace LoveCampus.Ui;

public class TiendaUI
{
    private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();
    private Usuario _usuarioActual;

    public TiendaUI(Usuario usuarioActual)
    {
        _usuarioActual = usuarioActual;
    }

    public void MenuTienda()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Tienda ===");
            var productos = _usuarioRepo.ObtenerProductosTienda();
            foreach (var producto in productos)
            {
                Console.WriteLine($"{producto.Id}. {producto.Nombre} - {producto.PrecioCapcoins} Capcoins");
            }
            Console.WriteLine("0. Regresar");
            Console.Write("Seleccione un producto para comprar: ");
            if (!int.TryParse(Console.ReadLine(), out int productoId) || productoId == 0)
                return;

            if (_usuarioRepo.RealizarCompra(_usuarioActual.Id, productoId))
                Console.WriteLine("Compra realizada con éxito.");
            else
                Console.WriteLine("No se pudo realizar la compra. Verifique sus capcoins.");
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
        }
    }
}