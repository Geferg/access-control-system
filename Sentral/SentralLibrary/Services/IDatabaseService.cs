using SentralLibrary.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Services;
public interface IDatabaseService
{
    UserDetailedData? GetUserById(string id);

    List<UserSimpleData> GetAllUsers();

    bool UserExists(string id);

    bool RemoveUser(string id);

    bool AddUser(UserDetailedData newUser);

    bool EditUser(string previousId, UserDetailedData newUserData);


    List<AlarmLogData> GetAlarmLogs(DateTime start, DateTime end);

    bool LogAlarm(AlarmLogData alarmLog);


    List<AccessLogData> GetAccessLogs(DateTime start, DateTime end);

    List<AccessLogData> GetDoorLogs(DateTime start, DateTime end, int doorNumber);

    bool LogAccess(AccessLogData accessLog);
}
