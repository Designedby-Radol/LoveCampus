using System;
using System.Collections.Generic;
using Dapper;
using MySql.Data.MySqlClient;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Interfaces;

namespace CampusLove.Infrastructure.Repositories
{
    public class InteractionRepository : IInteractionRepository
    {
        private readonly MySqlConnection _connection;

        public InteractionRepository(MySqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public Interaction GetById(int id)
        {
            return _connection.QueryFirstOrDefault<Interaction>(
                "SELECT * FROM log_interacciones WHERE id = @Id", 
                new { Id = id });
        }

        public IEnumerable<Interaction> GetAll()
        {
            return _connection.Query<Interaction>("SELECT * FROM log_interacciones");
        }

        public IEnumerable<Interaction> GetInteractionsByUserId(int userId)
        {
            return _connection.Query<Interaction>(
                @"SELECT id, usuario_origen_id as SourceUserId, usuario_destino_id as TargetUserId,
                  tipo as Type, fecha as InteractionDate
                  FROM log_interacciones
                  WHERE usuario_origen_id = @UserId",
                new { UserId = userId });
        }

        public bool HasLiked(int sourceUserId, int targetUserId)
        {
            return _connection.ExecuteScalar<bool>(
                @"SELECT COUNT(*) > 0
                  FROM log_interacciones
                  WHERE usuario_origen_id = @SourceId
                  AND usuario_destino_id = @TargetId
                  AND tipo = 'like'",
                new { SourceId = sourceUserId, TargetId = targetUserId });
        }

        public int CountDailyLikes(int userId)
        {
            return _connection.ExecuteScalar<int>(
                @"SELECT COUNT(*)
                  FROM log_interacciones
                  WHERE usuario_origen_id = @UserId
                  AND tipo = 'like'
                  AND DATE(fecha) = CURDATE()",
                new { UserId = userId });
        }

        public void Add(Interaction interaction)
        {
            _connection.Execute(
                @"INSERT INTO log_interacciones (usuario_origen_id, usuario_destino_id, tipo, fecha)
                  VALUES (@SourceUserId, @TargetUserId, @Type, @InteractionDate)",
                interaction);
        }

        public void Update(Interaction interaction)
        {
            _connection.Execute(
                @"UPDATE log_interacciones
                  SET usuario_origen_id = @SourceUserId,
                      usuario_destino_id = @TargetUserId,
                      tipo = @Type,
                      fecha = @InteractionDate
                  WHERE id = @Id",
                interaction);
        }

        public void Delete(int id)
        {
            _connection.Execute(
                "DELETE FROM log_interacciones WHERE id = @Id",
                new { Id = id });
        }
    }
}