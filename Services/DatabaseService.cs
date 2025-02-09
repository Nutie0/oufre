using Npgsql;
using System;

namespace registerAPI.Services;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    public NpgsqlConnection GetConnection() => new(_connectionString);
}
