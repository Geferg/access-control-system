using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class DatabaseConnection
{
    public const string test = "";
    private readonly NpgsqlConnection connection;
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

    public bool UserExists(string id)
    {
        bool result = false;
        string peekUserSql = $"SELECT * FROM peekuser('{id}')";

        try
        {
            connection.Open();
            using NpgsqlCommand command = new(peekUserSql, connection);
            using NpgsqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                string? exists = reader[0].ToString();
                if (exists == null)
                {
                    result = false;
                    connection.Close();
                }
                else if(!bool.TryParse(exists.ToLower(), out result))
                {
                    result = false;
                    connection.Close();
                }
                bool peek = result;
            }
        }
        catch (Exception ex)
        {
            TryLogMessage(ex.Message);
        }

        return result;
    }

    public void AddUser(UserData newUser)
    {
        bool success = false;
        string addUserSql = "SELECT * FROM adduser()";

    }

    public void RemoveUser(string id)
    {
        //TODO implement
    }

    public void UpdateUser(string id, UserData newUser)
    {
        //TODO implement
    }

    public UserData? GetUser(string id)
    {
        //UserData result;
        string getUserSql = $"SELECT * FROM getuser('{id}')";

        try
        {
            connection.Open();
            using NpgsqlCommand command = new(getUserSql, connection);
            using DataTable dataTable = new();
            dataTable.Load(command.ExecuteReader());

            DataRow row = dataTable.Rows[0];

            string? cardId = row[DatabaseColumns.IdCol].ToString();
            string? firstName = row[DatabaseColumns.FirstNameCol].ToString();
            string? lastName = row[DatabaseColumns.LastNameCol].ToString();
            string? email = row[DatabaseColumns.EmailCol].ToString();
            string? cardPin = row[DatabaseColumns.PinCol].ToString();
            DateTime start = (DateTime)row[DatabaseColumns.ValidStartCol];
            DateTime end = (DateTime)row[DatabaseColumns.ValidEndCol];

            if (cardId == null || firstName == null ||
                lastName == null || email == null || cardPin == null)
            {
                TryLogMessage($"error in database: field is null for user [{id}]");
                return null;
            }

            UserData userData = new(firstName, lastName, email, cardId, cardPin, start, end);
            return userData;
        }
        catch (Exception ex)
        {
            TryLogMessage(ex.Message);
        }
        finally
        {
            connection.Close();
        }

        return new UserData();
    }

    public List<(string id, string firstName, string lastName)> GetUserbase()
    {
        List<(string id, string firstName, string lastName)> result = new();
        string GetUserbaseSql = "SELECT * FROM getuserbase()";
        try
        {
            connection.Open();
            using NpgsqlCommand command = new(GetUserbaseSql, connection);
            using DataTable dataTable = new();
            dataTable.Load(command.ExecuteReader());

            DataRowCollection users = dataTable.Rows;

            foreach (DataRow user in users)
            {
                string? cardId = user[DatabaseColumns.IdCol].ToString();
                string? firstName = user[DatabaseColumns.FirstNameCol].ToString();
                string? lastName = user[DatabaseColumns.LastNameCol].ToString();

                //TODO rework null handling
                if(cardId != null && firstName != null && lastName != null)
                {
                    result.Add((cardId, firstName, lastName));
                }
            }

        }
        catch (Exception ex)
        {
            TryLogMessage(ex.Message);
        }
        finally
        {
            connection.Close();
        }
        return result;
    }

}