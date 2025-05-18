using System;
using System.Collections.Generic;
using Dapper;
using MySql.Data.MySqlClient;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Interfaces;

namespace CampusLove.Infrastructure.Repositories
{
    public class InterestRepository : IInterestRepository
    {
        private readonly MySqlConnection _connection;

        public InterestRepository(MySqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public Interest GetById(int id)
        {
            return _connection.QueryFirstOrDefault<Interest>(
                "SELECT * FROM intereses WHERE id = @Id", 
                new { Id = id });
        }

        public IEnumerable<Interest> GetAll()
        {
            return _connection.Query<Interest>(
                @"SELECT id, nombre as Name 
                  FROM intereses 
                  ORDER BY nombre");
        }

        public IEnumerable<Interest> GetInterestsByUserId(int userId)
        {
            return _connection.Query<Interest>(
                @"SELECT i.*
                  FROM intereses i
                  JOIN usuario_intereses ui ON i.id = ui.interes_id
                  WHERE ui.usuario_id = @UserId",
                new { UserId = userId });
        }

        public void SaveUserInterests(int userId, List<int> interestIds)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Delete existing user interests
                    _connection.Execute(
                        "DELETE FROM usuario_intereses WHERE usuario_id = @UserId",
                        new { UserId = userId },
                        transaction);

                    // Add new interests
                    foreach (var interestId in interestIds)
                    {
                        _connection.Execute(
                            @"INSERT INTO usuario_intereses (usuario_id, interes_id)
                              VALUES (@UserId, @InterestId)",
                            new { UserId = userId, InterestId = interestId },
                            transaction);
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

        public void Add(Interest interest)
        {
            _connection.Execute(
                @"INSERT INTO intereses (nombre)
                  VALUES (@Name)",
                interest);
        }

        public void Update(Interest interest)
        {
            _connection.Execute(
                @"UPDATE intereses
                  SET nombre = @Name
                  WHERE id = @Id",
                interest);
        }

        public void Delete(int id)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Delete user interests references
                    _connection.Execute(
                        "DELETE FROM usuario_intereses WHERE interes_id = @Id",
                        new { Id = id },
                        transaction);

                    // Delete interest
                    _connection.Execute(
                        "DELETE FROM intereses WHERE id = @Id",
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
    }
}