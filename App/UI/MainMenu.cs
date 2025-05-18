using System;
using CampusLove.App.Services;
using CampusLove.Domain.Entities;
using Spectre.Console;

namespace CampusLove.App.UI
{
    public class MainMenu
    {
        private User _currentUser;
        
        public void Show()
        {
            bool exit = false;

            while (!exit)
            {
                ApplicationUI.DisplayHeader("Campus Love");
                
                var choices = new List<string>();
                
                if (_currentUser == null)
                {
                    choices.Add("Iniciar Sesión");
                    choices.Add("Registrarse");
                }
                else
                {
                    choices.Add("Ver Perfiles");
                    choices.Add("Mis Matches");
                    choices.Add("Tienda");
                    choices.Add("Mi Perfil");
                    choices.Add("Estadísticas");
                    choices.Add("Cerrar Sesión");
                }
                
                choices.Add("Salir");
                
                var selection = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("¿Qué deseas hacer?")
                        .PageSize(10)
                        .AddChoices(choices)
                );
                
                switch (selection)
                {
                    case "Iniciar Sesión":
                        ShowLoginScreen();
                        break;
                    case "Registrarse":
                        ShowRegistrationScreen();
                        break;
                    case "Ver Perfiles":
                        ShowProfilesScreen();
                        break;
                    case "Mis Matches":
                        ShowMatchesScreen();
                        break;
                    case "Tienda":
                        ShowStoreScreen();
                        break;
                    case "Mi Perfil":
                        ShowMyProfileScreen();
                        break;
                    case "Estadísticas":
                        ShowStatisticsScreen();
                        break;
                    case "Cerrar Sesión":
                        _currentUser = null;
                        ApplicationUI.DisplaySuccess("Sesión cerrada exitosamente");
                        break;
                    case "Salir":
                        exit = true;
                        break;
                }
            }

            Console.Clear();
            AnsiConsole.Write(
                new FigletText("¡Gracias!")
                    .Centered()
                    .Color(Color.HotPink)
            );
            AnsiConsole.MarkupLine("[italic]¡Hasta pronto desde Campus Love![/]");
        }
        
        private void ShowLoginScreen()
        {
            ApplicationUI.DisplayHeader("Iniciar Sesión");
            
            try
            {
                string email = ApplicationUI.GetInput("Ingresa tu email");
                string password = AnsiConsole.Prompt(
                    new TextPrompt<string>("Ingresa tu contraseña:")
                        .Secret()
                );
                
                var userService = new UserService();
                var user = userService.Login(email, password);
                
                if (user != null)
                {
                    _currentUser = user;
                    ApplicationUI.DisplaySuccess($"¡Bienvenido de nuevo, {user.FormattedName}!");
                }
                else
                {
                    ApplicationUI.DisplayError("Email o contraseña inválidos");
                }
            }
            catch (Exception ex) when (ex is not ArgumentNullException)
            {
                ApplicationUI.DisplayError($"Error al iniciar sesión: {ex.Message}");
            }
            catch (ArgumentNullException)
            {
                ApplicationUI.DisplayError("Email o contraseña inválidos");
            }
        }
        
        private void ShowRegistrationScreen()
        {
            ApplicationUI.DisplayHeader("Registro");
            
            try
            {
                UserService userService = new UserService();
                
                string name = ApplicationUI.GetInput("Ingresa tu nombre");
                int age = ApplicationUI.GetIntInput("Ingresa tu edad", 18, 100);
                
                var genders = userService.GetAllGenders().ToList();
                var selectedGender = ApplicationUI.SelectGender("Selecciona tu género", genders);
                
                var careers = userService.GetAllCareers().ToList();
                var selectedCareer = ApplicationUI.SelectCareer("Selecciona tu carrera", careers);
                
                var interests = userService.GetAllInterests().ToList();
                var selectedInterests = ApplicationUI.MultiSelectOption("Selecciona tus intereses", interests);
                
                string email = ApplicationUI.GetInput("Ingresa tu email");
                
                string password = AnsiConsole.Prompt( 
                    new TextPrompt<string>("Ingresa tu contraseña:")
                        .Secret()
                );
                
                string confirmPassword = AnsiConsole.Prompt(
                    new TextPrompt<string>("Confirma tu contraseña:")
                        .Secret()
                );
                
                if (password != confirmPassword)
                {
                    ApplicationUI.DisplayError("Las contraseñas no coinciden");
                    return;
                }
                
                string profilePhrase = ApplicationUI.GetInput("Ingresa tu frase de perfil");
                
                var newUser = new User
                {
                    Name = name,
                    Age = age,
                    GenderId = selectedGender.Id,
                    Gender = selectedGender.Description,
                    CareerId = selectedCareer.Id,
                    Career = selectedCareer.Name,
                    Email = email,
                    Password = password,
                    ProfilePhrase = profilePhrase,
                    Interests = selectedInterests,
                    RegistrationDate = DateTime.Now
                };
                
                var registeredUser = userService.Register(newUser, selectedInterests.Select(i => i.Id).ToList());
                
                _currentUser = registeredUser;
                ApplicationUI.DisplaySuccess("¡Registro exitoso!");
            }
            catch (Exception ex)
            {
                ApplicationUI.DisplayError($"Error durante el registro: {ex.Message}");
            }
        }
        
        private void ShowProfilesScreen()
        {
            if (_currentUser == null)
            {
                ApplicationUI.DisplayError("Debes iniciar sesión para ver perfiles");
                return;
            }
            
            ApplicationUI.DisplayHeader("Explorar Perfiles");
            
            try
            {
                var interactionService = new InteractionService();
                var userService = new UserService();
                
                if (!interactionService.CanLike(_currentUser.Id))
                {
                    ApplicationUI.DisplayInfo($"No tienes más likes disponibles hoy. Visita la tienda para comprar más.");
                    
                    var visitStore = AnsiConsole.Confirm("¿Deseas visitar la tienda?");
                    if (visitStore)
                    {
                        ShowStoreScreen();
                        return;
                    }
                }
                
                var potentialMatches = userService.GetPotentialMatches(_currentUser.Id, 10).ToList();
                
                if (potentialMatches.Count == 0)
                {
                    ApplicationUI.DisplayInfo("No hay más perfiles para mostrar en este momento. ¡Vuelve más tarde!");
                    return;
                }
                
                foreach (var profile in potentialMatches)
                {
                    ApplicationUI.DisplayHeader("Perfil");
                    ApplicationUI.DisplayUserProfile(profile, detailed: true);
                    
                    AnsiConsole.MarkupLine($"[grey]Créditos restantes: {_currentUser.AvailableCredits}[/]");
                    
                    if (interactionService.CanLike(_currentUser.Id))
                    {
                        var choices = new[] { "Me gusta", "No me gusta", "Saltar", "Volver al menú" };
                        var action = ApplicationUI.SelectOption("¿Qué deseas hacer?", choices);
                        
                        switch (action)
                        {
                            case "Me gusta":
                                bool isMatch = interactionService.LikeUser(_currentUser.Id, profile.Id);
                                _currentUser = userService.GetById(_currentUser.Id);
                                
                                if (isMatch)
                                {
                                    ApplicationUI.DisplaySuccess($"¡Tienes un match con {profile.FormattedName}!");
                                }
                                else
                                {
                                    ApplicationUI.DisplaySuccess($"¡Le diste me gusta a {profile.FormattedName}!");
                                }
                                break;
                                
                            case "No me gusta":
                                interactionService.DislikeUser(_currentUser.Id, profile.Id);
                                ApplicationUI.DisplayInfo($"Pasaste de {profile.FormattedName}");
                                break;
                                
                            case "Volver al menú":
                                return;
                        }
                    }
                    else
                    {
                        ApplicationUI.DisplayInfo("No tienes más likes disponibles hoy");
                        var choices = new[] { "No me gusta", "Saltar", "Volver al menú" };
                        var action = ApplicationUI.SelectOption("¿Qué deseas hacer?", choices);
                        
                        switch (action)
                        {
                            case "No me gusta":
                                interactionService.DislikeUser(_currentUser.Id, profile.Id);
                                ApplicationUI.DisplayInfo($"Pasaste de {profile.FormattedName}");
                                break;
                                
                            case "Volver al menú":
                                return;
                        }
                    }
                }
                
                ApplicationUI.DisplayInfo("No hay más perfiles para mostrar en este momento. ¡Vuelve más tarde!");
            }
            catch (Exception ex)
            {
                ApplicationUI.DisplayError($"Error al ver perfiles: {ex.Message}");
            }
        }
        
        private void ShowMatchesScreen()
        {
            if (_currentUser == null)
            {
                ApplicationUI.DisplayError("Debes iniciar sesión para ver tus matches");
                return;
            }
            
            ApplicationUI.DisplayHeader("Mis Matches");
            
            try
            {
                var matchService = new MatchService();
                var matches = matchService.GetUserMatches(_currentUser.Id).ToList();
                
                if (matches.Count == 0)
                {
                    ApplicationUI.DisplayInfo("No tienes matches aún. ¡Sigue explorando perfiles!");
                    return;
                }
                
                var table = new Table()
                    .BorderColor(Color.HotPink)
                    .Border(TableBorder.Rounded);
                
                table.AddColumn(new TableColumn("Match").Centered());
                table.AddColumn(new TableColumn("Fecha").Centered());
                table.AddColumn(new TableColumn("Detalles").Centered());
                
                foreach (var match in matches)
                {
                    if (match.User1 == null || match.User2 == null) continue;

                    var otherUser = match.User1Id == _currentUser.Id ? match.User2 : match.User1;
                    var matchDate = match.MatchDate.ToString("dd/MM/yy");
                    var details = $"{otherUser.Age} | {otherUser.Career ?? "Sin carrera"}";
                    
                    table.AddRow(
                        otherUser.Name ?? "Usuario sin nombre",
                        matchDate,
                        details
                    );
                }
                
                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[dim]Presiona cualquier tecla para volver...[/]");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                ApplicationUI.DisplayError($"Error al cargar los matches: {ex.Message}");
            }
        }
        
        private void ShowStoreScreen()
        {
            if (_currentUser == null)
            {
                ApplicationUI.DisplayError("Debes iniciar sesión para acceder a la tienda");
                return;
            }
            
            ApplicationUI.DisplayHeader("Tienda");
            
            try
            {
                var storeService = new StoreService();
                var userService = new UserService();
                
                _currentUser = userService.GetById(_currentUser.Id);
                
                AnsiConsole.MarkupLine($"[green]Tus Capcoins: {_currentUser.FormattedCapcoins}[/]");
                AnsiConsole.MarkupLine($"[blue]Tus Créditos: {_currentUser.AvailableCredits}[/]");
                AnsiConsole.WriteLine();
                
                var items = storeService.GetAllItems().ToList();
                
                if (items.Count == 0)
                {
                    ApplicationUI.DisplayInfo("No hay artículos disponibles en la tienda en este momento.");
                    return;
                }
                
                var table = new Table()
                    .BorderColor(Color.Gold1)
                    .Border(TableBorder.Rounded);
                
                table.AddColumn(new TableColumn("ID").Centered());
                table.AddColumn(new TableColumn("Artículo").Centered());
                table.AddColumn(new TableColumn("Descripción").Centered());
                table.AddColumn(new TableColumn("Precio").Centered());
                table.AddColumn(new TableColumn("Cantidad").Centered());
                
                foreach (var item in items)
                {
                    if (item == null) continue;

                    table.AddRow(
                        item.Id.ToString(),
                        item.Name ?? "Sin nombre",
                        item.Description ?? "Sin descripción",
                        $"{item.FormattedPrice ?? "0"} Capcoins",
                        item.Quantity.ToString()
                    );
                }
                
                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();
                
                var choices = new List<string> { "Comprar artículo", "Volver al menú" };
                var action = ApplicationUI.SelectOption("¿Qué deseas hacer?", choices);
                
                if (action == "Comprar artículo")
                {
                    int itemId = ApplicationUI.GetIntInput("Ingresa el ID del artículo que deseas comprar", 1);
                    
                    var selectedItem = items.FirstOrDefault(i => i != null && i.Id == itemId);
                    if (selectedItem == null)
                    {
                        ApplicationUI.DisplayError("ID de artículo inválido");
                        return;
                    }
                    
                    var confirm = AnsiConsole.Confirm($"¿Confirmar compra de {selectedItem.Name ?? "artículo"} por {selectedItem.FormattedPrice ?? "0"} Capcoins?");
                    
                    if (confirm)
                    {
                        bool success = storeService.PurchaseItem(_currentUser.Id, itemId);
                        
                        if (success)
                        {
                            _currentUser = userService.GetById(_currentUser.Id);
                            ApplicationUI.DisplaySuccess($"¡Compra exitosa de {selectedItem.Name ?? "artículo"}!");
                            AnsiConsole.MarkupLine($"[green]Tus Capcoins: {_currentUser.FormattedCapcoins}[/]");
                            AnsiConsole.MarkupLine($"[blue]Tus Créditos: {_currentUser.AvailableCredits}[/]");
                        }
                        else
                        {
                            ApplicationUI.DisplayError("La compra falló. Es posible que no tengas suficientes Capcoins.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationUI.DisplayError($"Error al acceder a la tienda: {ex.Message}");
            }
        }
        
        private void ShowMyProfileScreen()
        {
            if (_currentUser == null)
            {
                ApplicationUI.DisplayError("Debes iniciar sesión para ver tu perfil");
                return;
            }
            
            ApplicationUI.DisplayHeader("Mi Perfil");
            
            var userService = new UserService();
            _currentUser = userService.GetById(_currentUser.Id);
            
            ApplicationUI.DisplayUserProfile(_currentUser, detailed: true);
            
            AnsiConsole.MarkupLine($"[green]Capcoins: {_currentUser.FormattedCapcoins}[/]");
            AnsiConsole.MarkupLine($"[grey]Miembro desde: {_currentUser.RegistrationDate:d}[/]");
            AnsiConsole.MarkupLine($"[grey]Último acceso: {(_currentUser.LastAccess?.ToString("g") ?? "N/A")}[/]");
            
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para volver...[/]");
            Console.ReadKey(true);
        }
        
        private void ShowStatisticsScreen()
        {
            ApplicationUI.DisplayHeader("Estadísticas");
            
            try
            {
                var userService = new UserService();
                var matchService = new MatchService();
                
                var mostLikedUsers = userService.GetMostLikedUsers(5);
                
                AnsiConsole.MarkupLine("[bold hotpink]Perfiles Más Populares[/]");
                var likedTable = new Table()
                    .BorderColor(Color.HotPink)
                    .Border(TableBorder.Rounded);
                
                likedTable.AddColumn(new TableColumn("Nombre").Centered());
                likedTable.AddColumn(new TableColumn("Edad").Centered());
                likedTable.AddColumn(new TableColumn("Carrera").Centered());
                
                foreach (var user in mostLikedUsers)
                {
                    likedTable.AddRow(
                        user.FormattedName,
                        user.Age.ToString(),
                        user.Career
                    );
                }
                
                AnsiConsole.Write(likedTable);
                AnsiConsole.WriteLine();
                
                var mostMatchedUsers = matchService.GetUsersWithMostMatches(5);
                
                AnsiConsole.MarkupLine("[bold cyan]Perfiles con Más Matches[/]");
                var matchedTable = new Table()
                    .BorderColor(Color.Cyan1)
                    .Border(TableBorder.Rounded);
                
                matchedTable.AddColumn(new TableColumn("Nombre").Centered());
                matchedTable.AddColumn(new TableColumn("Edad").Centered());
                matchedTable.AddColumn(new TableColumn("Carrera").Centered());
                
                foreach (var user in mostMatchedUsers)
                {
                    matchedTable.AddRow(
                        user.FormattedName,
                        user.Age.ToString(),
                        user.Career
                    );
                }
                
                AnsiConsole.Write(matchedTable);
                
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para volver...[/]");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                ApplicationUI.DisplayError($"Error al obtener estadísticas: {ex.Message}");
            }
        }
    }
}