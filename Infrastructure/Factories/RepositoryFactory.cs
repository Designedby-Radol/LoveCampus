using CampusLove.Domain.Interfaces;
using CampusLove.Infrastructure.Repositories;
using MySql.Data.MySqlClient;
using System;

namespace CampusLove.Infrastructure.Factories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly string _connectionString;

        public RepositoryFactory(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public T CreateRepository<T>() where T : class
        {
            var connection = new MySqlConnection(_connectionString);
            
            if (typeof(T) == typeof(IUserRepository))
            {
                return new UserRepository(connection) as T;
            }
            else if (typeof(T) == typeof(IMatchRepository))
            {
                return new MatchRepository(connection) as T;
            }
            else if (typeof(T) == typeof(IInteractionRepository))
            {
                return new InteractionRepository(connection) as T;
            }
            else if (typeof(T) == typeof(IInterestRepository))
            {
                return new InterestRepository(connection) as T;
            }
            else if (typeof(T) == typeof(ICareerRepository))
            {
                return new CareerRepository(connection) as T;
            }
            else if (typeof(T) == typeof(IGenderRepository))
            {
                return new GenderRepository(connection) as T;
            }
            else if (typeof(T) == typeof(IStoreRepository))
            {
                return new StoreRepository(connection) as T;
            }
            
            throw new ArgumentException($"Repository type {typeof(T).Name} is not supported.");
        }
    }
}