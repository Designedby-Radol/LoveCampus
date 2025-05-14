using LoveCampus.Application.UI;

namespace LoveCampus
{
    class Program
    {
        static async Task Main(string[] args) // Cambiar a async Task Main
        {
            try
            {
                var ui = new ConsolaUI();
                await ui.MostrarMenuPrincipal(); // Llamar al método correcto
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error fatal en la aplicación: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
            }
        }
    }
}