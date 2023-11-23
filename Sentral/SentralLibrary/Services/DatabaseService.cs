using SentralLibrary.Database.Processing;
using SentralLibrary.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Services;
public class DatabaseService : IDatabaseService
{
    private readonly DatabaseUserProcessing userProcessing;
    private readonly DatabaseAccessLogProcessing accessProcessing;
    private readonly DatabaseAlarmLogProcessing alarmProcessing;

    public DatabaseService(DatabaseUserProcessing userProcessing, DatabaseAccessLogProcessing accessProcessing, DatabaseAlarmLogProcessing alarmLogProcessing)
    {
        this.userProcessing = userProcessing;
        this.accessProcessing = accessProcessing;
        this.alarmProcessing = alarmLogProcessing;
    }

    public bool AddUser(UserDetailedData newUser)
    {
        return userProcessing.AddUser(newUser);
    }

    public bool EditUser(string previousId, UserDetailedData newUserData)
    {
        return userProcessing.EditUser(previousId, newUserData);
    }

    public List<UserSimpleData> GetAllUsers()
    {
        return userProcessing.GetUserbase();
    }

    public UserDetailedData? GetUserById(string id)
    {
        return userProcessing.GetUser(id);
    }

    public bool RemoveUser(string id)
    {
        return userProcessing.RemoveUser(id);
    }

    public bool UserExists(string id)
    {
        return userProcessing.UserExists(id);
    }

    public bool ValidateUser(string cardId, string cardPin)
    {
        return userProcessing.ValidateUser(cardId, cardPin);
    }

    public bool LogAlarm(AlarmLogData alarmLog)
    {
        return alarmProcessing.LogAlarm(alarmLog);
    }

    public List<AlarmLogData> GetAlarmLogs(DateTime start, DateTime end)
    {
        return alarmProcessing.GetAlarmLogs(start, end);
    }

    public bool LogAccess(AccessLogData accessLog)
    {
        return accessProcessing.LogAccess(accessLog);
    }

    public List<AccessLogData> GetAccessLogs(DateTime start, DateTime end)
    {
        return accessProcessing.GetAccessLogs(start, end);
    }

    public List<AccessLogData> GetDoorLogs(DateTime start, DateTime end, int doorNumber)
    {
        return accessProcessing.GetDoorLogs(start, end, doorNumber);
    }
}
