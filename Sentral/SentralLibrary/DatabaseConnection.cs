using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class DatabaseConnection
{
    private NpgsqlConnection connection;

    public DatabaseConnection(string hostIp, string username, string password, string database)
    {
        string connectionString = $"Host={hostIp};Username={username};Password={password};Database={database}";
        connection = new(connectionString);
    }

    public UserData? GetUserData(string id)
    {
        //TODO implement
        return new UserData();
    }

}