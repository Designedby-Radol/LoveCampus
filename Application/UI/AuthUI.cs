using LoveCampus.Infrastructure.Repositories;
using LoveCampus.Domain.Entities;

namespace LoveCampus.Ui;

public class AuthUI
{
    private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();
    private Usuario? _usuarioActual;

    public Usuario? IniciarSesion()
    {
        Console.Clear();
        Console.WriteLine("=== Iniciar Sesión ===");
        Console.Write("Nombre: ");
        var nombre = Console.ReadLine() ?? "";
        Console.Write("Contraseña: ");
        var password = Console.ReadLine() ?? "";

        _usuarioActual = _usuarioRepo.Autenticar(nombre, password);

        if (_usuarioActual == null)
        {
            Console.WriteLine("Credenciales incorrectas.");
            Console.ReadKey();
        }

        return _usuarioActual;
    }

    public void RegistrarUsuario()
    {
        try
        {
            Console.Clear();
            Console.WriteLine("=== Registro de Usuario ===");
            var usuario = new Usuario();

            Console.Write("Nombre: ");
            usuario.Nombre = Console.ReadLine() ?? "";

            Console.Write("Edad: ");
            if (!int.TryParse(Console.ReadLine(), out int edad) || edad < 18)
            {
                Console.WriteLine("Edad inválida. Debe ser mayor de 18.");
                Console.ReadKey();
                return;
            }
            usuario.Edad = edad;

            Console.Write("Género (1=Masculino, 2=Femenino): ");
            if (!int.TryParse(Console.ReadLine(), out int generoId) || (generoId != 1 && generoId != 2))
            {
                Console.WriteLine("Género inválido.");
                Console.ReadKey();
                return;
            }
            usuario.GeneroId = generoId;

            MostrarCarreras();
            Console.Write("Carrera (ID): ");
            if (!int.TryParse(Console.ReadLine(), out int carreraId))
            {
                Console.WriteLine("Carrera inválida.");
                Console.ReadKey();
                return;
            }
            usuario.CarreraId = carreraId;

            Console.Write("Email: ");
            usuario.Email = Console.ReadLine() ?? "";

            Console.Write("Contraseña: ");
            usuario.Password = Console.ReadLine() ?? "";

            Console.Write("Frase de perfil: ");
            usuario.FrasePerfil = Console.ReadLine() ?? "";

            usuario.CreditosDisponibles = 5;

            if (_usuarioRepo.Add(usuario))
            {
                Console.WriteLine("Usuario registrado exitosamente.");
            }
            else
            {
                Console.WriteLine("Error al registrar usuario.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
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
}