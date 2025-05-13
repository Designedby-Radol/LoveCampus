using System.Data;
using LoveCampus.Infrastructure.Config;
using MySql.Data.MySqlClient;

namespace LoveCampus.Infrastructure.Data;

public class DbContext : IDisposable
{
    private readonly MySqlConnection _connection;
    private bool _disposed;

    public DbContext()
    {
        _connection = new MySqlConnection(AppSettings.ConnectionString);
    }

    public MySqlConnection Connection
    {
        get
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            return _connection;
        }
    }

    public async Task<MySqlTransaction> BeginTransactionAsync()
    {
        return await Connection.BeginTransactionAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
                _connection.Dispose();
            }
            _disposed = true;
        }
    }

    ~DbContext()
    {
        Dispose(false);
    }
} 