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
    public readonly string HostIp;
    public readonly string HostPort;
    public readonly string DatabaseName;
    private readonly string connectionString;
    private UIConnection? uiConnection;


    public DatabaseConnection(string hostIp, string port, string username, string password, string database)
    {
        HostIp = hostIp;
        HostPort = port;
        DatabaseName = username;
        connectionString = $"Host={hostIp};Port={port};Username={username};Password={password};Database={database}";
    }

    public void AttachUIConnection(UIConnection uiConnection)
    {
        this.uiConnection = uiConnection;
    }

    public bool TestConnection()
    {
        try
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    protected virtual void TryLogMessage(string message)
    {
        uiConnection?.PutOnUI($"Debug (database): {message}");
    }

    public bool LogAlarm(DateTime timeOfAlarm, int doorNumber, string alarmType)
    {
        bool result = false;
        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_TIMEOFALARM, timeOfAlarm },
            {DbUserdataSchema.PARAM_DOORNUMBER, doorNumber },
            {DbUserdataSchema.PARAM_ALARMTYPE, alarmType }
        };

        try
        {
            DataTable dataTable = GetDataTable(DbUserdataSchema.FUNCTION_LOGALARM, parameters);

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

    public List<(DateTime time, int doorNumber, string alarmType)> GetAlarmLogs(DateTime start, DateTime end)
    {
        List<(DateTime time, int doorNumber, string alarmType)> result = new();

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_STARTDATE, start },
            {DbUserdataSchema.PARAM_ENDDATE, end }
        };

        try
        {
            DataTable dataTable = GetDataTable(DbUserdataSchema.FUNCTION_GETDOORACCESSLOGS, parameters);

            foreach (DataRow log in dataTable.Rows)
            {
                DateTime time = (DateTime)log[DbUserdataSchema.RETURN_TIMEOFALARM];
                int doorNumber = Convert.ToInt32(log[DbUserdataSchema.RETURN_DOORNUMBER].ToString());
                string? alarmType = log[DbUserdataSchema.RETURN_ALARMTYPE].ToString();

                if(alarmType != null)
                {
                    result.Add((time, doorNumber, alarmType));
                }
            }

        }
        catch (Exception ex)
        {
            TryLogMessage(ex.Message);
        }

        return result;
    }

    public List<(string id, bool approved, DateTime time, int doorNumber)> GetAccessLogs(DateTime start, DateTime end)
    {
        List<(string id, bool approved, DateTime time, int doorNumber)> result = new();

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_STARTDATE, start },
            {DbUserdataSchema.PARAM_ENDDATE, end }
        };

        try
        {
            DataTable dataTable = GetDataTable(DbUserdataSchema.FUNCTION_GETACCESSLOGS, parameters);

            foreach (DataRow log in dataTable.Rows)
            {
                string? id = log[DbUserdataSchema.RETURN_ID].ToString();
                bool approved = Convert.ToBoolean(log[DbUserdataSchema.RETURN_APPROVED].ToString());
                DateTime time = (DateTime)log[DbUserdataSchema.RETURN_TIMEOFENTRY];
                int doorNumber = Convert.ToInt32(log[DbUserdataSchema.RETURN_DOORNUMBER].ToString());

                if(id != null)
                {
                    result.Add((id, approved, time, doorNumber));
                }
            }

        }
        catch (Exception ex)
        {
            TryLogMessage(ex.Message);
        }

        return result;
    }

    public bool LogAccess(DateTime timeOfEntry, string cardId, bool approvedEntry, int doorNumber)
    {
        bool result = false;

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_TIMEOFENTRY, timeOfEntry },
            {DbUserdataSchema.PARAM_DOORNUMBER, doorNumber },
            {DbUserdataSchema.PARAM_APPROVEDENTRY, approvedEntry },
            {DbUserdataSchema.PARAM_ID , cardId }
        };

        try
        {
            DataTable dataTable = GetDataTable(DbUserdataSchema.FUNCTION_LOGACCESS, parameters);

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

    public bool UserExists(string id)
    {
        bool result = false;

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_ID, id}
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
            else
            {
                result = false;
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
        bool result = false;

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_ID, id}
        };

        try
        {
            DataTable dataTable = GetDataTable(DbUserdataSchema.FUNCTION_REMOVEUSER, parameters);

            if (dataTable.Rows.Count > 0)
            {
                DataRow row = dataTable.Rows[0];
                result = Convert.ToBoolean(row[0]);
            }
            else
            {
                result = false;
            }
        }
        catch (Exception ex)
        {
            TryLogMessage(ex.Message);
        }

        return result;
    }

    public bool UpdateUser(string previousId, UserData newUserData)
    {
        bool result = false;

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_PREVIOUSID, previousId},
            {DbUserdataSchema.PARAM_FIRSTNAME, newUserData.FirstName},
            {DbUserdataSchema.PARAM_LASTNAME, newUserData.LastName},
            {DbUserdataSchema.PARAM_EMAIL,  newUserData.Email},
            {DbUserdataSchema.PARAM_ID, newUserData.CardID },
            {DbUserdataSchema.PARAM_PIN, newUserData.CardPin },
            {DbUserdataSchema.PARAM_STARTVALIDITY, newUserData.ValidityPeriod.start },
            {DbUserdataSchema.PARAM_ENDVALIDITY,  newUserData.ValidityPeriod.end }
        };

        try
        {
            DataTable dataTable = GetDataTable(DbUserdataSchema.FUNCTION_UPDATEUSER, parameters);

            if (dataTable.Rows.Count > 0)
            {
                DataRow row = dataTable.Rows[0];
                result = Convert.ToBoolean(row[0]);
            }
            else
            {
                result = false;
            }
        }
        catch (Exception ex)
        {
            TryLogMessage(ex.Message);
        }

        return result;
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

            string? cardId = row[DbUserdataSchema.RETURN_ID].ToString();
            string? firstName = row[DbUserdataSchema.RETURN_FIRSTNAME].ToString();
            string? lastName = row[DbUserdataSchema.RETURN_LASTNAME].ToString();
            string? email = row[DbUserdataSchema.RETURN_EMAIL].ToString();
            string? cardPin = row[DbUserdataSchema.RETURN_PIN].ToString();
            DateTime start = (DateTime)row[DbUserdataSchema.RETURN_STARTVALIDITY];
            DateTime end = (DateTime)row[DbUserdataSchema.RETURN_ENDVALIDITY];

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
                string? cardId = user[DbUserdataSchema.RETURN_ID].ToString();
                string? firstName = user[DbUserdataSchema.RETURN_FIRSTNAME].ToString();
                string? lastName = user[DbUserdataSchema.RETURN_LASTNAME].ToString();

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
            DbUserdataSchema.FUNCTION_VALIDUSER,
            DbUserdataSchema.FUNCTION_LOGALARM,
            DbUserdataSchema.FUNCTION_LOGACCESS,
            DbUserdataSchema.FUNCTION_GETACCESSLOGS,
            DbUserdataSchema.FUNCTION_GETDOORACCESSLOGS,
            DbUserdataSchema.FUNCTION_GETALARMLOG,
            DbUserdataSchema.FUNCTION_GETSUSPICIOUSUSERS

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