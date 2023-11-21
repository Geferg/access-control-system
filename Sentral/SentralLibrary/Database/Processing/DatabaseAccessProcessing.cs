using SentralLibrary.DataClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Database.Processing;
public class DatabaseAccessProcessing
{
    private readonly DatabaseAccess databaseAccess;

    public DatabaseAccessProcessing(DatabaseAccess databaseAccess)
    {
        this.databaseAccess = databaseAccess;
    }

    public List<AccessLogData> GetAccessLogs(DateTime start, DateTime end)
    {
        List<AccessLogData> result = new();

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_STARTDATE, start },
            {DbUserdataSchema.PARAM_ENDDATE, end }
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.FUNCTION_GETACCESSLOGS, parameters);

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

        return result;
    }

    public List<AccessLogData> GetDoorLogs(DateTime start, DateTime end, int doorNumber)
    {
        List<AccessLogData> result = new();

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_STARTDATE, start },
            {DbUserdataSchema.PARAM_ENDDATE, end },
            {DbUserdataSchema.PARAM_DOORNUMBER, doorNumber}
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.FUNCTION_GETDOORACCESSLOGS, parameters);

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

        return result;
    }

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

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.FUNCTION_LOGACCESS, parameters);

        if (dataTable.Rows.Count > 0)
        {
            DataRow row = dataTable.Rows[0];
            result = Convert.ToBoolean(row[0]);
        }

        return result;
    }
}
