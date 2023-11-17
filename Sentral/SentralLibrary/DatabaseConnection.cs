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
        string query = $"SELECT {DatabaseColumns.IdCol}, " +
            $"{DatabaseColumns.FirstNameCol}, " +
            $"{DatabaseColumns.LastNameCol}, " +
            $"{DatabaseColumns.EmailCol}, " +
            $"{DatabaseColumns.PinCol} " +
            $"FROM {DatabaseColumns.TABLE_NAME} " +
            $"WHERE {DatabaseColumns.IdCol} = :id";

        try
        {
            connection.Open();
            using NpgsqlCommand cmd = new(query, connection);
            cmd.Parameters.AddWithValue(":id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string dbId = reader[DatabaseColumns.IdCol].ToString();
                string dbFirstName = reader[DatabaseColumns.FirstNameCol].ToString();
                string dbLastName = reader[DatabaseColumns.LastNameCol].ToString();
                string dbEmail = reader[DatabaseColumns.EmailCol].ToString();
                string dbPin = reader[DatabaseColumns.PinCol].ToString();

                return new UserData(dbFirstName, dbLastName, dbEmail, dbId, dbPin);
            }
        }
        catch (Exception ex)
        {

            throw;
        }

        return new UserData();
    }

    public List<(string id, string firstName, string lastName)> GetUserbase()
    {
        //TODO implement
        return new List<(string id, string firstName, string lastName)>();
    }

}