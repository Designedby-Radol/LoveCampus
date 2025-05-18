using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Interfaces;

namespace CampusLove.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MySqlConnection _connection;

        public UserRepository(MySqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public User GetById(int id)
        {
            var user = _connection.QueryFirstOrDefault<User>(
                @"SELECT u.id, u.nombre as Name, u.edad as Age, 
                         u.genero_id as GenderId, g.descripcion as Gender,
                         u.carrera_id as CareerId, c.nombre as Career,
                         u.email as Email, u.password as Password,
                         u.frase_perfil as ProfilePhrase,
                         u.creditos_disponibles as AvailableCredits,
                         u.capcoins as Capcoins,
                         u.fecha_registro as RegistrationDate,
                         u.ultimo_acceso as LastAccess,
                         u.rol as Role
                  FROM usuarios u
                  JOIN generos g ON u.genero_id = g.id
                  JOIN carreras c ON u.carrera_id = c.id
                  WHERE u.id = @Id", 
                  new { Id = id });

            if (user != null)
            {
                // Get user interests
                user.Interests = _connection.Query<Interest>(
                    @"SELECT i.id, i.nombre as Name 
                      FROM intereses i
                      JOIN usuario_intereses ui ON i.id = ui.interes_id
                      WHERE ui.usuario_id = @UserId", 
                    new { UserId = id }).ToList();
            }

            return user;
        }

        public User GetByEmail(string email)
        {
            var user = _connection.QueryFirstOrDefault<User>(
                @"SELECT u.*, g.descripcion as Gender, c.nombre as Career 
                  FROM usuarios u
                  JOIN generos g ON u.genero_id = g.id
                  JOIN carreras c ON u.carrera_id = c.id
                  WHERE u.email = @Email", new { Email = email });

            if (user != null)
            {
                // Get user interests
                user.Interests = _connection.Query<Interest>(
                    @"SELECT i.* FROM intereses i
                      JOIN usuario_intereses ui ON i.id = ui.interes_id
                      WHERE ui.usuario_id = @UserId", new { UserId = user.Id }).ToList();
            }

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            var users = _connection.Query<User>(
                @"SELECT u.*, g.descripcion as Gender, c.nombre as Career 
                  FROM usuarios u
                  JOIN generos g ON u.genero_id = g.id
                  JOIN carreras c ON u.carrera_id = c.id").ToList();

            foreach (var user in users)
            {
                // Get user interests
                user.Interests = _connection.Query<Interest>(
                    @"SELECT i.* FROM intereses i
                      JOIN usuario_intereses ui ON i.id = ui.interes_id
                      WHERE ui.usuario_id = @UserId", new { UserId = user.Id }).ToList();
            }

            return users;
        }

        public IEnumerable<User> GetPotentialMatches(int userId, int limit = 10)
        {
            // Get users who haven't interacted with the current user
            var potentialMatches = _connection.Query<User>(
                @"SELECT u.id, u.nombre as Name, u.edad as Age, 
                         u.genero_id as GenderId, g.descripcion as Gender,
                         u.carrera_id as CareerId, c.nombre as Career,
                         u.email as Email, u.password as Password,
                         u.frase_perfil as ProfilePhrase,
                         u.creditos_disponibles as AvailableCredits,
                         u.capcoins as Capcoins,
                         u.fecha_registro as RegistrationDate,
                         u.ultimo_acceso as LastAccess,
                         u.rol as Role
                  FROM usuarios u
                  JOIN generos g ON u.genero_id = g.id
                  JOIN carreras c ON u.carrera_id = c.id
                  WHERE u.id != @UserId
                  AND u.id NOT IN (
                      SELECT usuario_destino_id 
                      FROM log_interacciones 
                      WHERE usuario_origen_id = @UserId
                  )
                  LIMIT @Limit", 
                  new { UserId = userId, Limit = limit }).ToList();

            foreach (var user in potentialMatches)
            {
                // Get user interests
                user.Interests = _connection.Query<Interest>(
                    @"SELECT i.id, i.nombre as Name 
                      FROM intereses i
                      JOIN usuario_intereses ui ON i.id = ui.interes_id
                      WHERE ui.usuario_id = @UserId", 
                    new { UserId = user.Id }).ToList();
            }

            return potentialMatches;
        }

        public void Add(User user)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Insert user
                    var sql = @"INSERT INTO usuarios 
                              (nombre, edad, genero_id, carrera_id, email, password, frase_perfil, 
                               creditos_disponibles, capcoins, fecha_registro, rol) 
                              VALUES 
                              (@Name, @Age, @GenderId, @CareerId, @Email, @Password, @ProfilePhrase, 
                               @AvailableCredits, @Capcoins, @RegistrationDate, @Role);
                              SELECT LAST_INSERT_ID();";

                    var userId = _connection.ExecuteScalar<int>(sql, new
                    {
                        user.Name,
                        user.Age,
                        user.GenderId,
                        user.CareerId,
                        user.Email,
                        user.Password,
                        user.ProfilePhrase,
                        user.AvailableCredits,
                        user.Capcoins,
                        user.RegistrationDate,
                        user.Role
                    }, transaction);

                    user.Id = userId;

                    // Add user interests
                    if (user.Interests != null && user.Interests.Count > 0)
                    {
                        foreach (var interest in user.Interests)
                        {
                            _connection.Execute(
                                @"INSERT INTO usuario_intereses (usuario_id, interes_id) 
                                  VALUES (@UserId, @InterestId)",
                                new { UserId = userId, InterestId = interest.Id },
                                transaction);
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public void Update(User user)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Update user
                    _connection.Execute(
                        @"UPDATE usuarios 
                          SET nombre = @Name, 
                              edad = @Age, 
                              genero_id = @GenderId, 
                              carrera_id = @CareerId, 
                              email = @Email, 
                              password = @Password, 
                              frase_perfil = @ProfilePhrase, 
                              creditos_disponibles = @AvailableCredits, 
                              capcoins = @Capcoins, 
                              ultimo_acceso = @LastAccess, 
                              rol = @Role
                          WHERE id = @Id",
                        user, 
                        transaction);

                    // Update interests
                    if (user.Interests != null)
                    {
                        // Delete existing interests
                        _connection.Execute(
                            @"DELETE FROM usuario_intereses WHERE usuario_id = @UserId",
                            new { UserId = user.Id },
                            transaction);

                        // Add new interests
                        foreach (var interest in user.Interests)
                        {
                            _connection.Execute(
                                @"INSERT INTO usuario_intereses (usuario_id, interes_id) 
                                  VALUES (@UserId, @InterestId)",
                                new { UserId = user.Id, InterestId = interest.Id },
                                transaction);
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public void Delete(int id)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Delete user interests
                    _connection.Execute(
                        @"DELETE FROM usuario_intereses WHERE usuario_id = @UserId",
                        new { UserId = id },
                        transaction);

                    // Delete user interactions
                    _connection.Execute(
                        @"DELETE FROM log_interacciones 
                          WHERE usuario_origen_id = @UserId OR usuario_destino_id = @UserId",
                        new { UserId = id },
                        transaction);

                    // Delete user matches
                    _connection.Execute(
                        @"DELETE FROM matches 
                          WHERE usuario1_id = @UserId OR usuario2_id = @UserId",
                        new { UserId = id },
                        transaction);

                    // Delete user
                    _connection.Execute(
                        @"DELETE FROM usuarios WHERE id = @Id",
                        new { Id = id },
                        transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }

        public bool ValidateCredentials(string email, string password)
        {
            var user = _connection.QueryFirstOrDefault<User>(
                @"SELECT u.*, g.descripcion as Gender, c.nombre as Career 
                  FROM usuarios u
                  JOIN generos g ON u.genero_id = g.id
                  JOIN carreras c ON u.carrera_id = c.id
                  WHERE u.email = @Email AND u.password = @Password",
                new { Email = email, Password = password });

            if (user != null)
            {
                // Get user interests
                user.Interests = _connection.Query<Interest>(
                    @"SELECT i.* FROM intereses i
                      JOIN usuario_intereses ui ON i.id = ui.interes_id
                      WHERE ui.usuario_id = @UserId", 
                    new { UserId = user.Id }).ToList();
            }

            return user != null;
        }

        public void UpdateLastAccess(int userId)
        {
            _connection.Execute(
                @"UPDATE usuarios SET ultimo_acceso = @LastAccess WHERE id = @Id",
                new { LastAccess = DateTime.Now, Id = userId });

            // Log access
            _connection.Execute(
                @"INSERT INTO accesos_usuario (usuario_id, fecha_acceso) 
                  VALUES (@UserId, @AccessDate)",
                new { UserId = userId, AccessDate = DateTime.Now });
        }

        public void UpdateCredits(int userId, int newCreditAmount)
        {
            _connection.Execute(
                @"UPDATE usuarios SET creditos_disponibles = @Credits WHERE id = @Id",
                new { Credits = newCreditAmount, Id = userId });
        }

        public void UpdateCapcoins(int userId, int newCapcoinAmount)
        {
            _connection.Execute(
                @"UPDATE usuarios SET capcoins = @Capcoins WHERE id = @Id",
                new { Capcoins = newCapcoinAmount, Id = userId });
        }
    }
}