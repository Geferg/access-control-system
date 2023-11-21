using Npgsql;
using SentralLibrary.DataClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class DatabaseConnectionOld
{
    public readonly string HostIp;
    public readonly string HostPort;
    public readonly string DatabaseName;
    private readonly string connectionString;
    //private UIConnection? uiConnection;

    public DatabaseConnectionOld(string hostIp, string port, string username, string password, string database)
    {
        HostIp = hostIp;
        HostPort = port;
        DatabaseName = username;
        connectionString = $"Host={hostIp};Port={port};Username={username};Password={password};Database={database}";
    }
    /*

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

    // ========================= USER DATA ========================= //

    // Get
    //refactor 1
    public UserDetailedData? GetUser(string id)
    {
        UserDetailedData? result = new();

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_ID, id}
        };

        try
        {
            DataTable dataTable = GetDataTable(DbUserdataSchema.FUNCTION_GETUSER, parameters);
            DataRow row = dataTable.Rows[0];

            result.CardID = row[DbUserdataSchema.RETURN_ID].ToString();
            result.FirstName = row[DbUserdataSchema.RETURN_FIRSTNAME].ToString();
            result.LastName = row[DbUserdataSchema.RETURN_LASTNAME].ToString(); ;
            result.Email = row[DbUserdataSchema.RETURN_EMAIL].ToString();
            result.CardPin = row[DbUserdataSchema.RETURN_PIN].ToString();
            result.StartValidityTime = (DateTime)row[DbUserdataSchema.RETURN_STARTVALIDITY];
            result.EndValidityTime = (DateTime)row[DbUserdataSchema.RETURN_ENDVALIDITY];
        }
        catch (Exception ex)
        {
            TryLogMessage(ex.Message);
        }

        return result;
    }

    //refactor 1
    public List<UserSimpleData> GetUserbase()
    {
        List<UserSimpleData> result = new();
        try
        {
            DataTable table = GetDataTable(DbUserdataSchema.FUNCTION_GETUSERBASE, new Dictionary<string, object>());

            foreach (DataRow user in table.Rows)
            {
                UserSimpleData newData = new()
                {
                    CardID = user[DbUserdataSchema.RETURN_ID].ToString(),
                    FirstName = user[DbUserdataSchema.RETURN_FIRSTNAME].ToString(),
                    LastName = user[DbUserdataSchema.RETURN_LASTNAME].ToString()
                };

                result.Add(newData);
            }

        }
        catch (Exception ex)
        {
            TryLogMessage(ex.Message);
        }

        return result;
    }

    //refactor 0
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

    // Set
    //refactor 0
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

    //refactor 0
    public bool AddUser(UserDetailedData newUser)
    {
        bool result = false;

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_FIRSTNAME, newUser.FirstName},
            {DbUserdataSchema.PARAM_LASTNAME, newUser.LastName},
            {DbUserdataSchema.PARAM_EMAIL, newUser.Email},
            {DbUserdataSchema.PARAM_ID, newUser.CardID },
            {DbUserdataSchema.PARAM_PIN, newUser.CardPin },
            {DbUserdataSchema.PARAM_STARTVALIDITY, newUser.StartValidityTime },
            {DbUserdataSchema.PARAM_ENDVALIDITY, newUser.EndValidityTime }
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

    //refactor 0
    public bool EditUser(string previousId, UserDetailedData newUserData)
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
            {DbUserdataSchema.PARAM_STARTVALIDITY, newUserData.StartValidityTime },
            {DbUserdataSchema.PARAM_ENDVALIDITY,  newUserData.EndValidityTime }
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

    // ======================== CARD READER ======================== //

    // Get
    // refactor 0
    public bool ValidateUser(string id, string pin)
    {
        bool result = false;

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_ID, id},
            {DbUserdataSchema.PARAM_PIN, pin }
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

    // ======================== ACCESS LOG ======================== //

    // Get
    //refactor 1
    public List<AccessLogData> GetAccessLogs(DateTime start, DateTime end)
    {
        List<AccessLogData> result = new();

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
                AccessLogData newData = new()
                {
                    CardId = log[DbUserdataSchema.RETURN_ID].ToString(),
                    AccessGranted = Convert.ToBoolean(log[DbUserdataSchema.RETURN_APPROVED].ToString()),
                    Time = (DateTime)log[DbUserdataSchema.RETURN_TIMEOFENTRY],
                    DoorNumber = Convert.ToInt32(log[DbUserdataSchema.RETURN_DOORNUMBER].ToString())
                };

                result.Add(newData);
            }

        }
        catch (Exception ex)
        {
            TryLogMessage(ex.Message);
        }

        return result;
    }

    //refactor 1
    public List<AccessLogData> GetDoorLogs(DateTime start, DateTime end, int doorNumber)
    {
        List<AccessLogData> result = new();

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_STARTDATE, start },
            {DbUserdataSchema.PARAM_ENDDATE, end },
            {DbUserdataSchema.PARAM_DOORNUMBER, doorNumber}
        };

        try
        {
            DataTable dataTable = GetDataTable(DbUserdataSchema.FUNCTION_GETDOORACCESSLOGS, parameters);

            foreach (DataRow log in dataTable.Rows)
            {
                AccessLogData newData = new()
                {
                    CardId = log[DbUserdataSchema.RETURN_ID].ToString(),
                    AccessGranted = Convert.ToBoolean(log[DbUserdataSchema.RETURN_APPROVED].ToString()),
                    Time = (DateTime)log[DbUserdataSchema.RETURN_TIMEOFENTRY],
                    DoorNumber = doorNumber
                };

                result.Add(newData);
            }

        }
        catch (Exception ex)
        {
            TryLogMessage(ex.Message);
        }

        return result;
    }

    // Set
    //refactor 1
    public bool LogAccess(AccessLogData accessLog)
    {
        bool result = false;

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_TIMEOFENTRY, accessLog.Time },
            {DbUserdataSchema.PARAM_DOORNUMBER, accessLog.DoorNumber },
            {DbUserdataSchema.PARAM_APPROVEDENTRY, accessLog.AccessGranted },
            {DbUserdataSchema.PARAM_ID , accessLog.CardId ?? "" }
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

    // ======================== ALARM LOG ======================== //

    // Get
    //refactor 1
    public List<AlarmLogData> GetAlarmLogs(DateTime start, DateTime end)
    {
        List<AlarmLogData> result = new();

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
                AlarmLogData newData = new()
                {
                    Time = (DateTime)log[DbUserdataSchema.RETURN_TIMEOFALARM],
                    DoorNumber = Convert.ToInt32(log[DbUserdataSchema.RETURN_DOORNUMBER].ToString()),
                    AlarmType = log[DbUserdataSchema.RETURN_ALARMTYPE].ToString()
                };

                result.Add(newData);
            }

        }
        catch (Exception ex)
        {
            TryLogMessage(ex.Message);
        }

        return result;
    }

    // Set
    //refactor 1
    public bool LogAlarm(AlarmLogData alarmLog)
    {
        bool result = false;
        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_TIMEOFALARM, alarmLog.Time },
            {DbUserdataSchema.PARAM_DOORNUMBER, alarmLog.DoorNumber },
            {DbUserdataSchema.PARAM_ALARMTYPE, alarmLog.AlarmType ?? "" }
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

    // ========================= INTERNAL ========================= //

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

    // ======================== DEPRECATED ======================== //

    public void AttachUIConnection(UIConnection uiConnection)
    {
        //this.uiConnection = uiConnection;
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

    protected virtual void TryLogMessage(string message)
    {
        //uiConnection?.PutOnUI($"Debug (database): {message}");
    }
    */
}