using SentralLibrary.DataClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Database.Processing;
public class DatabaseAccessLogProcessing
{
    private readonly DatabaseAccess databaseAccess;

    public DatabaseAccessLogProcessing(DatabaseAccess databaseAccess)
    {
        this.databaseAccess = databaseAccess;
    }

    public List<AccessLogData> GetAccessLogs(DateTime start, DateTime end)
    {
        List<AccessLogData> result = new();

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.Parameter_DateTimeStart, start },
            {DbUserdataSchema.Parameter_DateTimeEnd, end }
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.Query_GetAccessReport, parameters);

        foreach (DataRow log in dataTable.Rows)
        {
            AccessLogData newData = new()
            {
                CardId = log[DbUserdataSchema.Return_Id].ToString(),
                AccessGranted = Convert.ToBoolean(log[DbUserdataSchema.Return_Approved].ToString()),
                Time = (DateTime)log[DbUserdataSchema.Return_TimeOfEntry],
                DoorNumber = Convert.ToInt32(log[DbUserdataSchema.Return_DoorNumber].ToString())
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
            {DbUserdataSchema.Parameter_DateTimeStart, start },
            {DbUserdataSchema.Parameter_DateTimeEnd, end },
            {DbUserdataSchema.Parameter_DoorNumber, doorNumber}
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.Query_GetDoorAccessReport, parameters);

        foreach (DataRow log in dataTable.Rows)
        {
            AccessLogData newData = new()
            {
                CardId = log[DbUserdataSchema.Return_Id].ToString(),
                AccessGranted = Convert.ToBoolean(log[DbUserdataSchema.Return_Approved].ToString()),
                Time = (DateTime)log[DbUserdataSchema.Return_TimeOfEntry],
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
            {DbUserdataSchema.Parameter_TimeOfEntry, accessLog.Time },
            {DbUserdataSchema.Parameter_DoorNumber, accessLog.DoorNumber },
            {DbUserdataSchema.Parameter_TimeOfEntry, accessLog.AccessGranted },
            {DbUserdataSchema.Parameter_CardId , accessLog.CardId ?? "" }
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.Query_LogAccess, parameters);

        if (dataTable.Rows.Count > 0)
        {
            DataRow row = dataTable.Rows[0];
            result = Convert.ToBoolean(row[0]);
        }

        return result;
    }
}