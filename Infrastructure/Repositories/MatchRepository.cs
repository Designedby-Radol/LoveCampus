using System;
using System.Collections.Generic;
using Dapper;
using MySql.Data.MySqlClient;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Interfaces;

namespace CampusLove.Infrastructure.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly MySqlConnection _connection;

        public MatchRepository(MySqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public Match GetById(int id)
        {
            return _connection.QueryFirstOrDefault<Match>(
                "SELECT * FROM matches WHERE id = @Id", 
                new { Id = id });
        }

        public IEnumerable<Match> GetAll()
        {
            return _connection.Query<Match>("SELECT * FROM matches");
        }

        public IEnumerable<Match> GetMatchesByUserId(int userId)
        {
            try
            {
                var matches = _connection.Query<Match, User, User, Match>(
                    @"SELECT m.id, m.fecha_match as MatchDate,
                             u1.id, u1.nombre as Name, u1.edad as Age, 
                             u1.genero_id as GenderId, g1.descripcion as Gender,
                             u1.carrera_id as CareerId, c1.nombre as Career,
                             u1.email as Email, u1.password as Password,
                             u1.frase_perfil as ProfilePhrase,
                             u1.creditos_disponibles as AvailableCredits,
                             u1.capcoins as Capcoins,
                             u1.fecha_registro as RegistrationDate,
                             u1.ultimo_acceso as LastAccess,
                             u1.rol as Role,
                             u2.id, u2.nombre as Name, u2.edad as Age, 
                             u2.genero_id as GenderId, g2.descripcion as Gender,
                             u2.carrera_id as CareerId, c2.nombre as Career,
                             u2.email as Email, u2.password as Password,
                             u2.frase_perfil as ProfilePhrase,
                             u2.creditos_disponibles as AvailableCredits,
                             u2.capcoins as Capcoins,
                             u2.fecha_registro as RegistrationDate,
                             u2.ultimo_acceso as LastAccess,
                             u2.rol as Role
                      FROM matches m
                      JOIN usuarios u1 ON m.usuario1_id = u1.id
                      JOIN usuarios u2 ON m.usuario2_id = u2.id
                      JOIN generos g1 ON u1.genero_id = g1.id
                      JOIN carreras c1 ON u1.carrera_id = c1.id
                      JOIN generos g2 ON u2.genero_id = g2.id
                      JOIN carreras c2 ON u2.carrera_id = c2.id
                      WHERE m.usuario1_id = @UserId OR m.usuario2_id = @UserId
                      ORDER BY m.fecha_match DESC",
                    (match, user1, user2) => {
                        match.User1 = user1;
                        match.User2 = user2;
                        return match;
                    },
                    new { UserId = userId },
                    splitOn: "id,id");

                // Verificar que los datos se mapearon correctamente
                foreach (var match in matches)
                {
                    if (match.User1 == null)
                    {
                        throw new InvalidOperationException($"Error al cargar el match ID {match.Id}: Usuario 1 no encontrado");
                    }
                    if (match.User2 == null)
                    {
                        throw new InvalidOperationException($"Error al cargar el match ID {match.Id}: Usuario 2 no encontrado");
                    }
                    if (string.IsNullOrEmpty(match.User1.Name))
                    {
                        throw new InvalidOperationException($"Error al cargar el match ID {match.Id}: Nombre del usuario 1 nulo o vacío");
                    }
                    if (string.IsNullOrEmpty(match.User2.Name))
                    {
                        throw new InvalidOperationException($"Error al cargar el match ID {match.Id}: Nombre del usuario 2 nulo o vacío");
                    }
                }

                return matches;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener los matches del usuario {userId}: {ex.Message}", ex);
            }
        }

        public bool IsMatch(int user1Id, int user2Id)
        {
            return _connection.ExecuteScalar<bool>(
                @"SELECT COUNT(*) > 0 
                  FROM log_interacciones 
                  WHERE usuario_origen_id = @User1Id 
                  AND usuario_destino_id = @User2Id 
                  AND tipo = 'like'
                  AND EXISTS (
                      SELECT 1 
                      FROM log_interacciones 
                      WHERE usuario_origen_id = @User2Id 
                      AND usuario_destino_id = @User1Id 
                      AND tipo = 'like'
                  )",
                new { User1Id = user1Id, User2Id = user2Id });
        }

        public void CreateMatch(int user1Id, int user2Id)
        {
            _connection.Execute(
                @"INSERT INTO matches (usuario1_id, usuario2_id, fecha_match) 
                  VALUES (@User1Id, @User2Id, @MatchDate)",
                new { User1Id = user1Id, User2Id = user2Id, MatchDate = DateTime.Now });
        }

        public void Add(Match match)
        {
            _connection.Execute(
                @"INSERT INTO matches (usuario1_id, usuario2_id, fecha_match) 
                  VALUES (@User1Id, @User2Id, @MatchDate)",
                match);
        }

        public void Update(Match match)
        {
            _connection.Execute(
                @"UPDATE matches 
                  SET usuario1_id = @User1Id, 
                      usuario2_id = @User2Id, 
                      fecha_match = @MatchDate 
                  WHERE id = @Id",
                match);
        }

        public void Delete(int id)
        {
            _connection.Execute(
                "DELETE FROM matches WHERE id = @Id", 
                new { Id = id });
        }
    }
}