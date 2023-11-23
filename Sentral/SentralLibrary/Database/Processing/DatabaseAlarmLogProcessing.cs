using SentralLibrary.DataClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Database.Processing;
public class DatabaseAlarmLogProcessing
{
    private readonly DatabaseAccess databaseAccess;

    public DatabaseAlarmLogProcessing(DatabaseAccess databaseAccess)
    {
        this.databaseAccess = databaseAccess;
    }

    public List<AlarmLogData> GetAlarmLogs(DateTime start, DateTime end)
    {
        List<AlarmLogData> result = new();

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.Parameter_DateTimeStart, start },
            {DbUserdataSchema.Parameter_DateTimeEnd, end }
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.Query_GetAlarmReport, parameters);

        foreach (DataRow log in dataTable.Rows)
        {
            AlarmLogData newData = new()
            {
                Time = (DateTime)log[DbUserdataSchema.Return_TimeOfAlarm],
                DoorNumber = Convert.ToInt32(log[DbUserdataSchema.Return_DoorNumber].ToString()),
                AlarmType = log[DbUserdataSchema.Return_AlarmType].ToString()
            };

            result.Add(newData);
        }

        return result;
    }

    public bool LogAlarm(AlarmLogData alarmLog)
    {
        bool result = false;
        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.Parameter_TimeOfAlarm, alarmLog.Time },
            {DbUserdataSchema.Parameter_DoorNumber, alarmLog.DoorNumber },
            {DbUserdataSchema.Parameter_AlarmType, alarmLog.AlarmType ?? "" }
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.Query_LogAlarm, parameters);

        if (dataTable.Rows.Count > 0)
        {
            DataRow row = dataTable.Rows[0];
            result = Convert.ToBoolean(row[0]);
        }

        return result;
    }
}