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
            {DbUserdataSchema.PARAM_STARTDATE, start },
            {DbUserdataSchema.PARAM_ENDDATE, end }
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.FUNCTION_GETDOORACCESSLOGS, parameters);

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

        return result;
    }

    public bool LogAlarm(AlarmLogData alarmLog)
    {
        bool result = false;
        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_TIMEOFALARM, alarmLog.Time },
            {DbUserdataSchema.PARAM_DOORNUMBER, alarmLog.DoorNumber },
            {DbUserdataSchema.PARAM_ALARMTYPE, alarmLog.AlarmType ?? "" }
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.FUNCTION_LOGALARM, parameters);

        if (dataTable.Rows.Count > 0)
        {
            DataRow row = dataTable.Rows[0];
            result = Convert.ToBoolean(row[0]);
        }

        return result;
    }
}
