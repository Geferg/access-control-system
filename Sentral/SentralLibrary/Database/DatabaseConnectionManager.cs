using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Database;
public class DatabaseConnectionManager
{
    public string HostIp { get; }
    public string HostPort { get; }
    public string DatabaseName { get; }

    private readonly string connectionString;

    public DatabaseConnectionManager(string hostIp, string database, string hostPort, string username, string password)
    {
        HostIp = hostIp;
        HostPort = hostPort;
        DatabaseName = database;
        connectionString = $"Host={hostIp};Port={hostPort};Username={username};Password={password};Database={database}";
    }

    public NpgsqlConnection GetOpenConnection()
    {
        NpgsqlConnection connection = new(connectionString);
        connection.Open();
        return connection;
    }

    public bool TestConnection()
    {
        try
        {
            using var connection = GetOpenConnection();
            // Optionally, execute a simple query like "SELECT 1" to test the connection
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
