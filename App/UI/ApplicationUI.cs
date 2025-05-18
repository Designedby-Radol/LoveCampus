using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using CampusLove.Domain.Entities;

namespace CampusLove.App.UI
{
    public static class ApplicationUI
    {
        public static void DisplayWelcomeScreen()
        {
            Console.Clear();
            
            var logoFiglet = new FigletText("Campus Love")
                .Centered()
                .Color(Color.HotPink);
            
            var taglineFiglet = new FigletText("Where is Love")
                .Centered()
                .Color(Color.DeepSkyBlue1);
            
            AnsiConsole.Write(
                new Panel(
                    new Markup("[bold]:hearts: ¡Bienvenido a [hotpink]Campus Love[/], una app de citas para estudiantes! :hearts:[/]")
                )
                .Expand()
                .BorderColor(Color.HotPink)
            );
            
            AnsiConsole.Write(logoFiglet);
            AnsiConsole.Write(taglineFiglet);
            
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("Presiona cualquier tecla para continuar...");
            Console.ReadKey(true);
        }
        
        public static void DisplayHeader(string title)
        {
            Console.Clear();
            
            var headerFiglet = new FigletText(title)
                .Centered()
                .Color(Color.HotPink);
            
            AnsiConsole.Write(headerFiglet);
            AnsiConsole.WriteLine();
        }
        
        public static void DisplayError(string message)
        {
            AnsiConsole.Write(
                new Panel(
                    new Markup($"[bold red]{message}[/]")
                )
                .BorderColor(Color.Red)
                .Expand()
            );
            
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
            Console.ReadKey(true);
        }
        
        public static void DisplaySuccess(string message)
        {
            AnsiConsole.Write(
                new Panel(
                    new Markup($"[bold green]{message}[/]")
                )
                .BorderColor(Color.Green)
                .Expand()
            );
            
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
            Console.ReadKey(true);
        }
        
        public static void DisplayInfo(string message)
        {
            AnsiConsole.Write(
                new Panel(
                    new Markup($"[bold blue]{message}[/]")
                )
                .BorderColor(Color.Blue)
                .Expand()
            );
        }
        
        public static void DisplayUserProfile(CampusLove.Domain.Entities.User user, bool detailed = false)
        {
            var panel = new Panel(
                new Markup($"[bold]{user.FormattedName}, {user.Age}[/]\n[italic]{user.Career ?? "Sin carrera especificada"}[/]")
            )
            .BorderColor(Color.HotPink)
            .Header($"[b]Perfil #{user.Id}[/]");
            
            AnsiConsole.Write(panel);
            
            if (detailed)
            {
                var table = new Table()
                    .BorderColor(Color.HotPink)
                    .Border(TableBorder.Rounded);
                
                table.AddColumn(new TableColumn("Detalle").Centered());
                table.AddColumn(new TableColumn("Información").Centered());
                
                table.AddRow("Género", user.Gender ?? "No especificado");
                
                string interestsText;
                if (user.Interests == null || !user.Interests.Any())
                {
                    interestsText = "Sin intereses especificados";
                }
                else
                {
                    var validInterests = user.Interests.Where(i => !string.IsNullOrEmpty(i.Name)).ToList();
                    interestsText = validInterests.Any() 
                        ? string.Join(", ", validInterests.Select(i => i.Name))
                        : "Sin intereses especificados";
                }
                table.AddRow("Intereses", interestsText);
                
                table.AddRow("Perfil", string.IsNullOrEmpty(user.ProfilePhrase) ? "Sin frase de perfil" : user.ProfilePhrase);
                table.AddRow("Créditos", user.FormattedCredits);
                
                AnsiConsole.Write(table);
            }
        }
        
        public static string GetInput(string prompt)
        {
            var input = AnsiConsole.Ask<string>($"[cyan]{prompt}:[/]");
            return !string.IsNullOrWhiteSpace(input) ? input : throw new ArgumentException("El campo no puede estar vacío");
        }
        
        public static int GetIntInput(string prompt, int minValue = 0, int maxValue = int.MaxValue)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<int>($"[cyan]{prompt}:[/]")
                    .Validate(age => 
                    {
                        if (age < minValue)
                            return ValidationResult.Error($"El valor debe ser al menos {minValue}");
                        if (age > maxValue)
                            return ValidationResult.Error($"El valor debe ser como máximo {maxValue}");
                        return ValidationResult.Success();
                    })
            );
        }
        
        public static T SelectOption<T>(string title, IEnumerable<T> options) where T : class
        {
            if (options == null || !options.Any())
                throw new ArgumentException("La lista de opciones no puede estar vacía");

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<T>()
                    .Title($"[cyan]{title}[/]")
                    .PageSize(10)
                    .AddChoices(options)
                    .UseConverter(obj => 
                    {
                        if (obj is Gender gender)
                            return gender.Description ?? string.Empty;
                        if (obj is Career career)
                            return career.Name ?? string.Empty;
                        if (obj is Interest interest)
                            return interest.Name ?? string.Empty;
                        return obj?.ToString() ?? "Sin nombre";
                    })
            );
            
            return selection;
        }
        
        public static Career SelectCareer(string title, IEnumerable<Career> careers)
        {
            if (careers == null || !careers.Any())
                throw new ArgumentException("La lista de carreras no puede estar vacía");

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<Career>()
                    .Title($"[cyan]{title}[/]")
                    .PageSize(10)
                    .AddChoices(careers)
                    .UseConverter(career => career.Name ?? "Sin nombre")
            );
            
            return selection;
        }
        
        public static Gender SelectGender(string title, IEnumerable<Gender> genders)
        {
            if (genders == null || !genders.Any())
                throw new ArgumentException("La lista de géneros no puede estar vacía");

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<Gender>()
                    .Title($"[cyan]{title}[/]")
                    .PageSize(10)
                    .AddChoices(genders)
                    .UseConverter(gender => gender.Description ?? "Sin descripción")
            );
            
            return selection;
        }
        
        public static List<T> MultiSelectOption<T>(string title, IEnumerable<T> options) where T : class
        {
            if (options == null || !options.Any())
                throw new ArgumentException("La lista de opciones no puede estar vacía");

            var selections = AnsiConsole.Prompt(
                new MultiSelectionPrompt<T>()
                    .Title($"[cyan]{title} (usa [grey]espacio[/] para seleccionar, [grey]enter[/] para confirmar)[/]")
                    .PageSize(10)
                    .InstructionsText("[grey](Presiona [blue]<espacio>[/] para alternar una selección, [green]<enter>[/] para aceptar)[/]")
                    .Required()
                    .AddChoices(options)
                    .UseConverter(obj => 
                    {
                        if (obj is Interest interest)
                            return interest.Name ?? "Sin nombre";
                        if (obj is Gender gender)
                            return gender.Description ?? "Sin descripción";
                        if (obj is Career career)
                            return career.Name ?? "Sin nombre";
                        return obj?.ToString() ?? "Sin nombre";
                    })
            );
            
            return selections.ToList();
        }
    }
}