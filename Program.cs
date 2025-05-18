using System;
using CampusLove.App.UI;
using CampusLove.Infrastructure.Config;

namespace CampusLove
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set up database connection
            try
            {
                DatabaseConfig.Initialize();
                Console.WriteLine("Database connection established successfully.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to connect to database: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.Title = "Campus Love...Where is Love";
            
            // Display welcome screen
            ApplicationUI.DisplayWelcomeScreen();
            
            // Initialize main menu
            MainMenu mainMenu = new MainMenu();
            mainMenu.Show();
        }
    }
}