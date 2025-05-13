using System.Data;
using Dapper;
using LoveCampus.Domain.Interfaces;
using LoveCampus.Infrastructure.Data;
using MySql.Data.MySqlClient;

namespace LoveCampus.Infrastructure.Repositories;

public abstract class Repository<T> : IRepository<T> where T : class
{
    protected readonly string TableName;
    protected readonly string IdColumnName;

    protected Repository(string tableName, string idColumnName = "Id")
    {
        TableName = tableName;
        IdColumnName = idColumnName;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        using var dbContext = new DbContext();
        return await dbContext.Connection.QueryAsync<T>($"SELECT * FROM {TableName}");
    }

    public virtual async Task<T?> GetByIdAsync(object id)
    {
        using var dbContext = new DbContext();
        return await dbContext.Connection.QueryFirstOrDefaultAsync<T>(
            $"SELECT * FROM {TableName} WHERE {IdColumnName} = @Id",
            new { Id = id });
    }

    public virtual async Task<bool> AddAsync(T entity)
    {
        using var dbContext = new DbContext();
        var properties = typeof(T).GetProperties()
            .Where(p => p.Name != IdColumnName)
            .ToList();

        var columns = string.Join(", ", properties.Select(p => p.Name));
        var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));

        var sql = $"INSERT INTO {TableName} ({columns}) VALUES ({values})";
        
        var affected = await dbContext.Connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public virtual async Task<bool> UpdateAsync(T entity)
    {
        using var dbContext = new DbContext();
        var properties = typeof(T).GetProperties()
            .Where(p => p.Name != IdColumnName)
            .ToList();

        var updates = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
        var sql = $"UPDATE {TableName} SET {updates} WHERE {IdColumnName} = @{IdColumnName}";

        var affected = await dbContext.Connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public virtual async Task<bool> DeleteAsync(object id)
    {
        using var dbContext = new DbContext();
        var sql = $"DELETE FROM {TableName} WHERE {IdColumnName} = @Id";
        var affected = await dbContext.Connection.ExecuteAsync(sql, new { Id = id });
        return affected > 0;
    }

    public virtual async Task<int> CountAsync()
    {
        using var dbContext = new DbContext();
        return await dbContext.Connection.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {TableName}");
    }

    public virtual async Task<bool> ExistsAsync(object id)
    {
        using var dbContext = new DbContext();
        var sql = $"SELECT COUNT(*) FROM {TableName} WHERE {IdColumnName} = @Id";
        var count = await dbContext.Connection.ExecuteScalarAsync<int>(sql, new { Id = id });
        return count > 0;
    }

    protected async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<MySqlConnection, MySqlTransaction, Task<TResult>> action)
    {
        using var dbContext = new DbContext();
        using var transaction = await dbContext.BeginTransactionAsync();
        try
        {
            var result = await action(dbContext.Connection, transaction);
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
} 