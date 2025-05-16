using LoveCampus.Infrastructure.Repositories;
using LoveCampus.Domain.Entities;

namespace LoveCampus.Ui;

public class UsuarioUI
{
    private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();
    private Usuario _usuarioActual;

    public UsuarioUI(Usuario usuarioActual)
    {
        _usuarioActual = usuarioActual;
    }

    public void MenuUsuario()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Bienvenido, {_usuarioActual.Nombre}!");
            Console.WriteLine("1. Ver perfiles y dar Like/Dislike");
            Console.WriteLine("2. Ver mis coincidencias (matches)");
            Console.WriteLine("3. Gestionar mi información");
            Console.WriteLine("4. Acceder a la tienda");
            Console.WriteLine("5. Cerrar sesión");
            Console.Write("Seleccione una opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    VerPerfiles();
                    break;
                case "2":
                    VerMatches();
                    break;
                case "3":
                    GestionarInformacionUsuario();
                    break;
                case "4":
                    new TiendaUI(_usuarioActual).MenuTienda();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void GestionarInformacionUsuario()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Gestión de Información ===");
            Console.WriteLine($"1. Editar nombre (Actual: {_usuarioActual.Nombre})");
            Console.WriteLine($"2. Editar edad (Actual: {_usuarioActual.Edad})");
            Console.WriteLine($"3. Editar género (Actual: {(_usuarioActual.GeneroId == 1 ? "Masculino" : "Femenino")})");
            Console.WriteLine($"4. Editar email (Actual: {_usuarioActual.Email})");
            Console.WriteLine($"5. Editar contraseña");
            Console.WriteLine($"6. Editar frase de perfil (Actual: {_usuarioActual.FrasePerfil})");
            Console.WriteLine("8. Regresar");
            Console.Write("Seleccione una opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    Console.Write("Nuevo nombre: ");
                    var nuevoNombre = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(nuevoNombre))
                    {
                        _usuarioActual.Nombre = nuevoNombre;
                        GuardarCambios();
                    }
                    break;
                case "2":
                    Console.Write("Nueva edad: ");
                    if (int.TryParse(Console.ReadLine(), out int nuevaEdad) && nuevaEdad >= 18)
                    {
                        _usuarioActual.Edad = nuevaEdad;
                        GuardarCambios();
                    }
                    else
                    {
                        Console.WriteLine("Edad inválida. Debe ser mayor o igual a 18.");
                    }
                    break;
                case "3":
                    Console.Write("Nuevo género (1=Masculino, 2=Femenino): ");
                    if (int.TryParse(Console.ReadLine(), out int nuevoGenero) && (nuevoGenero == 1 || nuevoGenero == 2))
                    {
                        _usuarioActual.GeneroId = nuevoGenero;
                        GuardarCambios();
                    }
                    else
                    {
                        Console.WriteLine("Género inválido.");
                    }
                    break;
                case "5":
                    Console.Write("Nuevo email: ");
                    var nuevoEmail = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(nuevoEmail))
                    {
                        _usuarioActual.Email = nuevoEmail;
                        GuardarCambios();
                    }
                    break;
                case "6":
                    Console.Write("Nueva contraseña: ");
                    var nuevaPassword = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(nuevaPassword))
                    {
                        _usuarioActual.Password = nuevaPassword;
                        GuardarCambios();
                    }
                    break;
                case "7":
                    Console.Write("Nueva frase de perfil: ");
                    var nuevaFrase = Console.ReadLine();
                    _usuarioActual.FrasePerfil = nuevaFrase;
                    GuardarCambios();
                    break;
                case "8":
                    return;
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
        }
    }

    private void MostrarCarreras()
    {
        try
        {
            var carreras = _usuarioRepo.ObtenerCarreras();
            Console.WriteLine("=== Carreras Disponibles ===");
            foreach (var carrera in carreras)
            {
                Console.WriteLine($"{carrera.Id}: {carrera.Nombre}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener las carreras: {ex.Message}");
        }
    }

    private void GuardarCambios()
    {
        if (_usuarioRepo.Update(_usuarioActual))
        {
            Console.WriteLine("Información actualizada correctamente.");
        }
        else
        {
            Console.WriteLine("Error al actualizar la información.");
        }
    }

    private void VerPerfiles()
    {
        // Implementación similar a la actual
    }

    private void VerMatches()
    {
        // Implementación similar a la actual
    }
}