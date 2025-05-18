using System;
using System.Collections.Generic;
using Dapper;
using MySql.Data.MySqlClient;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Interfaces;

namespace CampusLove.Infrastructure.Repositories
{
    public class GenderRepository : IGenderRepository
    {
        private readonly MySqlConnection _connection;

        public GenderRepository(MySqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public Gender GetById(int id)
        {
            return _connection.QueryFirstOrDefault<Gender>(
                "SELECT id, descripcion as Description FROM generos WHERE id = @Id", 
                new { Id = id });
        }

        public IEnumerable<Gender> GetAll()
        {
            return _connection.Query<Gender>("SELECT id, descripcion as Description FROM generos");
        }

        public void Add(Gender gender)
        {
            _connection.Execute(
                "INSERT INTO generos (descripcion) VALUES (@Description)",
                gender);
        }

        public void Update(Gender gender)
        {
            _connection.Execute(
                "UPDATE generos SET descripcion = @Description WHERE id = @Id",
                gender);
        }

        public void Delete(int id)
        {
            // Check if gender is being used by any users
            var isUsed = _connection.ExecuteScalar<bool>(
                "SELECT COUNT(*) > 0 FROM usuarios WHERE genero_id = @Id",
                new { Id = id });
            
            if (isUsed)
            {
                throw new InvalidOperationException("Cannot delete gender as it is being used by users.");
            }

            _connection.Execute(
                "DELETE FROM generos WHERE id = @Id",
                new { Id = id });
        }
    }
}