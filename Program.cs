using LoveCampus.Ui;

class Program
{
    static void Main(string[] args)
    {
        var authUI = new AuthUI();
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
                    var usuario = authUI.IniciarSesion();
                    if (usuario != null)
                    {
                        if (usuario.Rol == "admin")
                            new AdminUI().MenuAdministrador();
                        else
                            new UsuarioUI(usuario).MenuUsuario();
                    }
                    break;
                case "2":
                    authUI.RegistrarUsuario();
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
}