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

        Dictionary<string, object> parameters = new()
        {
            {"id", id}
        };


        try
        {
            DataTable dataTable = GetDataTable(DbUserdataSchema.FUNCTION_USEREXISTS, parameters);

            if (dataTable.Rows.Count > 0)
            {
                DataRow row = dataTable.Rows[0];
                result = Convert.ToBoolean(row[0]);
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
        bool result = false;

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_FIRSTNAME, newUser.FirstName},
            {DbUserdataSchema.PARAM_LASTNAME, newUser.LastName},
            {DbUserdataSchema.PARAM_EMAIL, newUser.Email},
            {DbUserdataSchema.PARAM_ID, newUser.CardID },
            {DbUserdataSchema.PARAM_PIN, newUser.CardPin },
            {DbUserdataSchema.PARAM_STARTVALIDITY, newUser.ValidityPeriod.start },
            {DbUserdataSchema.PARAM_ENDVALIDITY, newUser.ValidityPeriod.end }
        };

        try
        {
            DataTable dataTable = GetDataTable(DbUserdataSchema.FUNCTION_ADDUSER, parameters);

            if (dataTable.Rows.Count > 0)
            {
                DataRow row = dataTable.Rows[0];
                result = Convert.ToBoolean(row[0]);
            }

        }
        catch (Exception ex)
        {
            TryLogMessage(ex.Message);
        }

        return result;
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
        UserData? result = null;

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_ID, id}
        };

        try
        {
            //TODO verify
            DataTable dataTable = GetDataTable(DbUserdataSchema.FUNCTION_GETUSER, parameters);

            DataRow row = dataTable.Rows[0];

            string? cardId = row[DbUserdataSchema.COLUMN_ID].ToString();
            string? firstName = row[DbUserdataSchema.COLUMN_FIRSTNAME].ToString();
            string? lastName = row[DbUserdataSchema.COLUMN_LASTNAME].ToString();
            string? email = row[DbUserdataSchema.COLUMN_EMAIL].ToString();
            string? cardPin = row[DbUserdataSchema.COLUMN_PIN].ToString();
            DateTime start = (DateTime)row[DbUserdataSchema.COLUMN_STARTVALIDITY];
            DateTime end = (DateTime)row[DbUserdataSchema.COLUMN_ENDVALIDITY];

            if (cardId != null && firstName != null &&
                lastName != null && email != null && cardPin != null)
            {
                result = new(firstName, lastName, email, cardId, cardPin, start, end);
            }
            else
            {
                //TODO clean up nullable return, replace with error msg in log
                result = null;
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