using MySql.Data.MySqlClient;
using CampusLove.Infrastructure.Factories;
using CampusLove.Domain.Interfaces;
using System;
using Dapper;

namespace CampusLove.Infrastructure.Config
{
    public static class DatabaseConfig
    {
        private static string _connectionString = string.Empty;
        private static IRepositoryFactory? _repositoryFactory;

        public static IRepositoryFactory RepositoryFactory => _repositoryFactory ?? throw new InvalidOperationException("RepositoryFactory no ha sido inicializado");

        public static void Initialize()
        {
            string server = "localhost";
            string database = "campus_love";
            string uid = "root";
            string password = "11358";  
            string port = "5506";

            _connectionString = $"Server={server};Database={database};Uid={uid};Pwd={password};port={port};";
            
            // Test connection
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
            }
            
            // Initialize the repository factory
            _repositoryFactory = new RepositoryFactory(_connectionString);
        }

        public static MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}