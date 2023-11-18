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
    //private readonly NpgsqlConnection connection_;
    private readonly string connectionString;
    private UILogger? logger;

    public DatabaseConnection(string hostIp, string port, string username, string password, string database)
    {
        connectionString = $"Host={hostIp};Port={port};Username={username};Password={password};Database={database}";
        //connection_ = new(connectionString);
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
            connection_.Open();
            using NpgsqlCommand command = new(peekUserSql, connection_);
            using NpgsqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                string? exists = reader[0].ToString();
                if (exists == null)
                {
                    result = false;
                    connection_.Close();
                }
                else if(!bool.TryParse(exists.ToLower(), out result))
                {
                    result = false;
                    connection_.Close();
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

    public bool AddUser(UserData newUser)
    {
        return false;
    }

    public bool RemoveUser(string id)
    {
        //TODO implement
        return false;
    }

    public bool UpdateUser(string id, UserData newUser)
    {
        return false;
    }

    public UserData? GetUser(string id)
    {
        UserData result = new();

        Dictionary<string, object> parameters = new()
        {
            {"id", id}
        };

        //string getUserSql = $"SELECT * FROM getuser('{id}')";
        try
        {
            //TODO verify
            DataTable dataTable = GetDataTable(DbUserdataSchema.FUNCTION_GETUSER, parameters);

            DataRow row = dataTable.Rows[0];

            string? cardId = row[DatabaseColumns.IdCol].ToString();
            string? firstName = row[DatabaseColumns.FirstNameCol].ToString();
            string? lastName = row[DatabaseColumns.LastNameCol].ToString();
            string? email = row[DatabaseColumns.EmailCol].ToString();
            string? cardPin = row[DatabaseColumns.PinCol].ToString();
            DateTime start = (DateTime)row[DatabaseColumns.ValidStartCol];
            DateTime end = (DateTime)row[DatabaseColumns.ValidEndCol];

            if (cardId != null && firstName != null &&
                lastName != null && email != null && cardPin != null)
            {
                result = new(firstName, lastName, email, cardId, cardPin, start, end);
            }
        }
        catch (Exception ex)
        {
            TryLogMessage(ex.Message);
        }

        return result;
    }

    public List<(string id, string firstName, string lastName)> GetUserbase()
    {
        List<(string id, string firstName, string lastName)> result = new();
        try
        {
            DataTable table = GetDataTable(DbUserdataSchema.FUNCTION_GETUSERBASE, new Dictionary<string, object>());

            foreach (DataRow user in table.Rows)
            {
                string? cardId = user[DbUserdataSchema.COLUMN_ID].ToString();
                string? firstName = user[DbUserdataSchema.COLUMN_FIRSTNAME].ToString();
                string? lastName = user[DbUserdataSchema.COLUMN_LASTNAME].ToString();

                //TODO rework null handling if needed
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

        return result;
    }

    private DataTable GetDataTable(string function, Dictionary<string, object> parameters)
    {
        string sql = $"SELECT * FROM {GetValidatedFunctionName(function)}(";
        if(parameters.Any())
        {
            var parameterPlaceholders = parameters.OrderBy(p => p.Key).Select(p => "@" + p.Key);
            sql += string.Join(", ", parameterPlaceholders);
        }
        sql += ")";

        DataTable dataTable = new();

        try
        {
            using NpgsqlConnection connection = new(connectionString);
            using NpgsqlCommand command = new(sql, connection);

            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            connection.Open();
            dataTable.Load(command.ExecuteReader());
        }
        catch (Exception ex)
        {
            TryLogMessage(ex.Message);
        }

        return dataTable;
    }

    private static string GetValidatedFunctionName(string function)
    {
        var validFunctions = new HashSet<string>
        {
            DbUserdataSchema.FUNCTION_GETUSERBASE,
            DbUserdataSchema.FUNCTION_GETUSER,
            DbUserdataSchema.FUNCTION_REMOVEUSER,
            DbUserdataSchema.FUNCTION_UPDATEUSER,
            DbUserdataSchema.FUNCTION_ADDUSER,
            DbUserdataSchema.FUNCTION_USEREXISTS,
            DbUserdataSchema.FUNCTION_VALIDUSER
        };

        if (validFunctions.Contains(function))
        {
            return function;
        }
        else
        {
            throw new ArgumentException("Invalid function name", nameof(function));
        }
    }


}