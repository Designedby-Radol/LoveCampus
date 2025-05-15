using System;
using System.Globalization;
using LoveCampus.Infrastructure.Repositories;
using LoveCampus.Domain.Entities;

namespace LoveCampus.Ui
{
    public class ConsoleUI
    {
        private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();
        private Usuario? _usuarioActual = null;

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== LoveCampus ===");
                Console.WriteLine("1. Registrarse como nuevo usuario");
                Console.WriteLine("2. Iniciar sesión");
                Console.WriteLine("3. Salir");
                Console.Write("Seleccione una opción: ");
                var opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        RegistrarUsuario();
                        break;
                    case "2":
                        IniciarSesion();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void RegistrarUsuario()
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

                // Mostrar carreras antes de pedir el ID
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

        private void IniciarSesion()
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
                return;
            }

            if (_usuarioActual.Rol == "admin")
                MenuAdministrador();
            else
                MenuUsuario();
        }

        private void MenuUsuario()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Bienvenido, {_usuarioActual?.Nombre}!");
                Console.WriteLine("1. Ver perfiles y dar Like/Dislike");
                Console.WriteLine("2. Ver mis coincidencias (matches)");
                Console.WriteLine("3. Ver estadísticas");
                Console.WriteLine("4. Cerrar sesión");
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
                        VerEstadisticas();
                        break;
                    case "4":
                        _usuarioActual = null;
                        return;
                    default:
                        Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void MenuAdministrador()
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
                        _usuarioActual = null;
                        return;
                    default:
                        Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void VerPerfiles()
        {
            try
            {
                var perfiles = _usuarioRepo.ObtenerPerfilesDisponibles(_usuarioActual!.Id);
                foreach (var perfil in perfiles)
                {
                    Console.Clear();
                    Console.WriteLine($"Nombre: {perfil.Nombre}");
                    Console.WriteLine($"Edad: {perfil.Edad}");
                    Console.WriteLine($"Género: {perfil.GeneroDescripcion}");
                    Console.WriteLine($"Carrera: {perfil.CarreraNombre}");
                    Console.WriteLine($"Frase: {perfil.FrasePerfil}");
                    Console.WriteLine($"Intereses: {string.Join(", ", perfil.Intereses ?? new List<string>())}");
                    Console.WriteLine("¿Like (L) o Dislike (D)? (S para salir)");
                    var op = Console.ReadLine()?.ToUpper();
                    if (op == "L")
                    {
                        if (_usuarioRepo.RegistrarInteraccion(_usuarioActual.Id, perfil.Id, "like"))
                            Console.WriteLine("¡Like registrado!");
                        else
                            Console.WriteLine("No puedes dar más likes hoy.");
                    }
                    else if (op == "D")
                    {
                        _usuarioRepo.RegistrarInteraccion(_usuarioActual.Id, perfil.Id, "dislike");
                        Console.WriteLine("Dislike registrado.");
                    }
                    else if (op == "S")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Opción inválida.");
                    }
                    Console.WriteLine("Presione una tecla para continuar...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadKey();
            }
        }

        private void VerMatches()
        {
            try
            {
                var matches = _usuarioRepo.ObtenerMatches(_usuarioActual!.Id);
                Console.Clear();
                Console.WriteLine("=== Tus Matches ===");
                foreach (var match in matches)
                {
                    Console.WriteLine($"Nombre: {match.Nombre} | Carrera: {match.CarreraNombre} | Género: {match.GeneroDescripcion}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
        }

        private void VerEstadisticas()
        {
            // Aquí puedes implementar estadísticas usando LINQ sobre los datos obtenidos del repositorio
            Console.Clear();
            Console.WriteLine("=== Estadísticas del Sistema ===");
            Console.WriteLine("Funcionalidad pendiente de implementación.");
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
        }

        private void VerDatosUsuario()
        {
            Console.Write("Ingrese el ID del usuario: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                Console.ReadKey();
                return;
            }
            var usuario = _usuarioRepo.ObtenerPorId(id);
            if (usuario == null)
            {
                Console.WriteLine("Usuario no encontrado.");
            }
            else
            {
                Console.WriteLine($"Nombre: {usuario.Nombre}");
                Console.WriteLine($"Email: {usuario.Email}");
                Console.WriteLine($"Edad: {usuario.Edad}");
                Console.WriteLine($"Género: {usuario.GeneroDescripcion}");
                Console.WriteLine($"Carrera: {usuario.CarreraNombre}");
                Console.WriteLine($"Frase: {usuario.FrasePerfil}");
                Console.WriteLine($"Créditos: {usuario.CreditosDisponibles}");
            }
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
        }

        private void VerLogInteracciones()
        {
            Console.Write("Ingrese el ID del usuario: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                Console.ReadKey();
                return;
            }
            // Aquí deberías implementar el método en el repositorio para obtener el log de interacciones
            Console.WriteLine("Funcionalidad pendiente de implementación.");
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
        }

        private void VerHistorialAccesos()
        {
            Console.Write("Ingrese el ID del usuario: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                Console.ReadKey();
                return;
            }
            // Aquí deberías implementar el método en el repositorio para obtener el historial de accesos
            Console.WriteLine("Funcionalidad pendiente de implementación.");
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
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
        }
    }
}