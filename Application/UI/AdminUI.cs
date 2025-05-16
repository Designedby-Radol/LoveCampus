using LoveCampus.Infrastructure.Repositories;

namespace LoveCampus.Ui;

public class AdminUI
{
    private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();

    public void MenuAdministrador()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Panel de Administrador ===");
            Console.WriteLine("1. Ver datos de usuario");
            Console.WriteLine("2. Ver log de interacciones");
            Console.WriteLine("3. Ver historial de accesos");
            Console.WriteLine("4. Cerrar sesión");
            Console.Write("Seleccione una opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    VerDatosUsuario();
                    break;
                case "2":
                    VerLogInteracciones();
                    break;
                case "3":
                    VerHistorialAccesos();
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void VerDatosUsuario()
    {
        // Implementación similar a la actual
    }

    private void VerLogInteracciones()
    {
        // Implementación similar a la actual
    }

    private void VerHistorialAccesos()
    {
        // Implementación similar a la actual
    }
}