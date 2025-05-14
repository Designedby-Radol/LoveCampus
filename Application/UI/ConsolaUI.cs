using System;
using System.Threading.Tasks;

namespace LoveCampus.Application.UI;

public class ConsolaUI
{
    private const string TITULO_APP = "💘 CAMPUS LOVE 💘";
    private const string SEPARADOR = "===================================";

    public async Task MostrarMenuPrincipal()
    {
        while (true)
        {
            Console.Clear();
            ImprimirTitulo(TITULO_APP, ConsoleColor.Magenta);
            Console.WriteLine("\n1. Iniciar Sesión");
            Console.WriteLine("2. Registrarse");
            Console.WriteLine("3. Salir\n");

            Console.Write("Seleccione una opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    await MostrarInicioSesion();
                    break;
                case "2":
                    await MostrarRegistro();
                    break;
                case "3":
                    return;
                default:
                    MostrarError("Opción no válida");
                    break;
            }
        }
    }

    private async Task MostrarInicioSesion()
    {
        Console.Clear();
        ImprimirTitulo("🔐 INICIAR SESIÓN", ConsoleColor.Cyan);

        Console.Write("\nEmail: ");
        var email = Console.ReadLine();
        Console.Write("Contraseña: ");
        var password = LeerPassword();

        Console.WriteLine("\n[1] Iniciar Sesión");
        Console.WriteLine("[2] Volver al Menú Principal");

        var opcion = Console.ReadLine();
        if (opcion == "2") return;

        // TODO: Implementar lógica de inicio de sesión
        await Task.Delay(1000); // Simulación de proceso
    }

    private async Task MostrarRegistro()
    {
        Console.Clear();
        ImprimirTitulo("📝 REGISTRO DE USUARIO", ConsoleColor.Cyan);

        Console.Write("\nNombre completo: ");
        var nombre = Console.ReadLine();
        Console.Write("Edad: ");
        var edad = Console.ReadLine();
        Console.Write("Email: ");
        var email = Console.ReadLine();
        Console.Write("Contraseña: ");
        var password = LeerPassword();
        Console.Write("Confirmar contraseña: ");
        var confirmPassword = LeerPassword();

        // TODO: Implementar lógica de registro
        await Task.Delay(1000); // Simulación de proceso
    }

    public async Task MostrarPaginaPrincipal(string nombreUsuario)
    {
        while (true)
        {
            Console.Clear();
            ImprimirTitulo($"🏠 PÁGINA PRINCIPAL - {nombreUsuario}", ConsoleColor.Yellow);

            // TODO: Obtener perfil aleatorio de la base de datos
            Console.WriteLine("\n👤 Nombre: María García, 21 años");
            Console.WriteLine("🎓 Carrera: Psicología");
            Console.WriteLine("💬 Intereses: Música, Baile, Yoga");
            Console.WriteLine("💖 Bio: \"Amante de la psicología y la música\"\n");

            Console.WriteLine("¿Qué deseas hacer?");
            Console.WriteLine("[1] ❤️ Like");
            Console.WriteLine("[2] ❌ Dislike");
            Console.WriteLine("[3] 👤 Ver Siguiente Perfil");
            Console.WriteLine("[4] 💬 Ver Mis Matches");
            Console.WriteLine("[5] 🚪 Cerrar Sesión");

            var opcion = Console.ReadLine();
            if (opcion == "5") break;

            // TODO: Implementar lógica de interacción
            await Task.Delay(1000); // Simulación de proceso
        }
    }

    public void MostrarMatch(string nombreMatch)
    {
        Console.Clear();
        ImprimirTitulo("🎉 ¡HAS HECHO MATCH! 🎉", ConsoleColor.Green);

        Console.WriteLine($"\n💘 Tú y {nombreMatch} se gustaron mutuamente.");
        Console.WriteLine("Puedes empezar a chatear ahora.\n");

        Console.WriteLine("[1] Ver Otro Perfil");
        Console.WriteLine("[2] Ir al Chat");
        Console.WriteLine("[3] Volver al Menú Principal");

        Console.ReadKey();
    }

    public void MostrarNoMatch()
    {
        Console.Clear();
        ImprimirTitulo("😢 No fue un Match 😢", ConsoleColor.Red);

        Console.WriteLine("\nSigue explorando... ¡El amor está cerca!\n");

        Console.WriteLine("[1] Ver Otro Perfil");
        Console.WriteLine("[2] Volver al Menú Principal");

        Console.ReadKey();
    }

    private void ImprimirTitulo(string titulo, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(SEPARADOR);
        Console.WriteLine(titulo);
        Console.WriteLine(SEPARADOR);
        Console.ResetColor();
    }

    private void MostrarError(string mensaje)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n❌ Error: {mensaje}");
        Console.ResetColor();
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    private string LeerPassword()
    {
        var password = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);
            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Substring(0, password.Length - 1);
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }
}
