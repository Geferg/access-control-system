using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class DatabaseConnection
{
    private NpgsqlConnection connection;
    private UILogger? logger;

    public DatabaseConnection(string hostIp, string port, string username, string password, string database)
    {
        string connectionString = $"Host={hostIp};Port={port};Username={username};Password={password};Database={database}";
        connection = new(connectionString);
    }

    public void AttachLogger(UILogger logger)
    {
        this.logger = logger;
    }

    protected virtual void TryLogMessage(string message)
    {
        logger?.LogMessage(message);
    }

    public static void AddUser(UserData newUser)
    {
        //TODO implement
    }

    public static void RemoveUser(string cardId)
    {
        //TODO implement
    }

    public static void UpdateUser(string cardId, UserData newUser)
    {
        //TODO implement
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

                connection.Close();

                return new UserData(dbFirstName, dbLastName, dbEmail, dbId, dbPin);
            }
        }
        catch (Exception ex)
        {

            throw;
        }
        finally
        {
            connection.Close();
        }

        return new UserData();
    }

    public List<(string id, string firstName, string lastName)> GetUserbase()
    {
        //TODO implement
        return new List<(string id, string firstName, string lastName)>();
    }

}