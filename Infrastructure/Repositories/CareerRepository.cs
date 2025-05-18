using System;
using System.Collections.Generic;
using Dapper;
using MySql.Data.MySqlClient;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Interfaces;

namespace CampusLove.Infrastructure.Repositories
{
    public class CareerRepository : ICareerRepository
    {
        private readonly MySqlConnection _connection;

        public CareerRepository(MySqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public Career GetById(int id)
        {
            return _connection.QueryFirstOrDefault<Career>(
                "SELECT id, nombre as Name FROM carreras WHERE id = @Id", 
                new { Id = id });
        }

        public IEnumerable<Career> GetAll()
        {
            return _connection.Query<Career>("SELECT id, nombre as Name FROM carreras");
        }

        public void Add(Career career)
        {
            _connection.Execute(
                "INSERT INTO carreras (nombre) VALUES (@Name)",
                career);
        }

        public void Update(Career career)
        {
            _connection.Execute(
                "UPDATE carreras SET nombre = @Name WHERE id = @Id",
                career);
        }

        public void Delete(int id)
        {
            // Check if career is being used by any users
            var isUsed = _connection.ExecuteScalar<bool>(
                "SELECT COUNT(*) > 0 FROM usuarios WHERE carrera_id = @Id",
                new { Id = id });
            
            if (isUsed)
            {
                throw new InvalidOperationException("Cannot delete career as it is being used by users.");
            }

            _connection.Execute(
                "DELETE FROM carreras WHERE id = @Id",
                new { Id = id });
        }
    }
}