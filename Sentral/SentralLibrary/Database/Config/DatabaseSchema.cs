using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Database.Config;
internal static class DatabaseSchema
{
    // ================= PARAMETERS =================
    // User data
    public const string Parameter_CardId = "CardId";
    public const string Parameter_CardPin = "CardPin";
    public const string Parameter_Email = "Email";
    public const string Parameter_FirstName = "FirstName";
    public const string Parameter_LastName = "LastName";
    public const string Parameter_StartValidity = "ValidEnd";
    public const string Parameter_EndValidity = "ValidStart";

    public const string Parameter_OldCardId = "PreviousCardId";

    // Access log
    public const string Parameter_ApprovedEntry = "ApprovedEntry";
    public const string Parameter_TimeOfEntry = "TimeOfEntry";

    public const string Parameter_DoorNumber = "DoorNumber";
    public const string Parameter_DateTimeStart = "StartDate";
    public const string Parameter_DateTimeEnd = "EndDate";

    // Alarm log
    public const string Parameter_TimeOfAlarm = "TimeOfAlarm";
    public const string Parameter_AlarmType = "AlarmType";

    // ================= QUERIES =================

    // User data
    public const string Query_AddUser = $"select * from adduser(" +
        $"@{Parameter_CardId}," +
        $"@{Parameter_CardPin}," +
        $"@{Parameter_Email}," +
        $"@{Parameter_FirstName}," +
        $"@{Parameter_LastName}," +
        $"@{Parameter_EndValidity}," +
        $"@{Parameter_StartValidity})";

    public const string Query_EditUser = $"select * from updateuser(" +
        $"@{Parameter_CardId}," +
        $"@{Parameter_CardPin}," +
        $"@{Parameter_Email}," +
        $"@{Parameter_FirstName}," +
        $"@{Parameter_LastName}," +
        $"@{Parameter_OldCardId}," +
        $"@{Parameter_EndValidity}," +
        $"@{Parameter_StartValidity})";

    public const string Query_GetUserbase = "select * from GetUserbase()";

    public const string Query_GetUser = $"select * from GetUser(@{Parameter_CardId})";

    public const string Query_PeekUser = $"select * from peekuser(@{Parameter_CardId})";

    public const string Query_RemoveUser = $"select * from RemoveUser(@{Parameter_CardId})";

    public const string Query_ValidateUser = $"select * from ValidateUser(" +
        $"@{Parameter_CardId}," +
        $"@{Parameter_CardPin})";

    // Access log
    public const string Query_LogAccess = $"select * from AddAccessLog(" +
        $"@{Parameter_CardId}," +
        $"@{Parameter_ApprovedEntry}," +
        $"@{Parameter_TimeOfEntry}," +
        $"@{Parameter_DoorNumber})";

    public const string Query_GetDoorAccessReport = $"select * from DoorReport(" +
        $"@{Parameter_DoorNumber}," +
        $"@{Parameter_DateTimeStart}," +
        $"@{Parameter_DateTimeEnd})";

    public const string Query_GetAccessReport = $"select * from AccessReport(" +
        $"@{Parameter_DateTimeStart}," +
        $"@{Parameter_DateTimeEnd})";

    public const string Query_GetSuspiciousUsers = "select * from SuspiciousUsers()";

    // Alarm log
    public const string Query_LogAlarm = $"select * from AddAlarmLog(" +
        $"@{Parameter_TimeOfAlarm}," +
        $"@{Parameter_DoorNumber}," +
        $"@{Parameter_AlarmType})";

    public const string Query_GetAlarmReport = $"select * from AlarmReport(" +
        $"@{Parameter_DateTimeStart}," +
        $"@{Parameter_DateTimeEnd})";

    // ================= RETURNS =================

    public const string Return_FirstName = "firstName";
    public const string Return_LastName = "lastName";
    public const string Return_Email = "emailAddress";
    public const string Return_Id = "cardId";
    public const string Return_Pin = "cardPin";
    public const string Return_StartValidity = "validStart";
    public const string Return_EndValidity = "validEnd";
    public const string Return_Approved = "approvedentry";
    public const string Return_TimeOfAlarm = "timeOfAlarm";
    public const string Return_TimeOfEntry = "timeOfEntry";
    public const string Return_DoorNumber = "doorNumber";
    public const string Return_AlarmType = "alarmType";

    // ================= DEPRECATED =================
    // Columns
    /*
    public const string COLUMN_FIRSTNAME = "first_name";
    public const string COLUMN_LASTNAME = "last_name";
    public const string COLUMN_EMAIL = "email_address";
    public const string COLUMN_ID = "card_id";
    public const string COLUMN_PIN = "card_pin";
    public const string COLUMN_STARTVALIDITY = "valid_start";
    public const string COLUMN_ENDVALIDITY = "valid_end";

    public const string RETURN_FIRSTNAME = "firstName";
    public const string RETURN_LASTNAME = "lastName";
    public const string RETURN_EMAIL = "emailAddress";
    public const string RETURN_ID = "cardId";
    public const string RETURN_PIN = "cardPin";
    public const string RETURN_STARTVALIDITY = "validStart";
    public const string RETURN_ENDVALIDITY = "validEnd";
    public const string RETURN_APPROVED = "approved";
    public const string RETURN_TIMEOFALARM = "timeOfAlarm";
    public const string RETURN_TIMEOFENTRY = "timeOfEntry";
    public const string RETURN_DOORNUMBER = "doorNumber";
    public const string RETURN_ALARMTYPE = "alarmType";

    public const string PARAM_FIRSTNAME = "firstName_";
    public const string PARAM_LASTNAME = "lastName_";
    public const string PARAM_EMAIL = "emailAddress_";
    public const string PARAM_ID = "cardId_";
    public const string PARAM_PIN = "cardPin_";
    public const string PARAM_STARTVALIDITY = "validStart_";
    public const string PARAM_ENDVALIDITY = "validEnd_";
    public const string PARAM_PREVIOUSID = "previousCardId_";
    public const string PARAM_TIMEOFALARM = "TimeOfAlarm_";
    public const string PARAM_DOORNUMBER = "DoorNumber_";
    public const string PARAM_ALARMTYPE = "AlarmType_";
    public const string PARAM_TIMEOFENTRY = "TimeOfEntry_";
    public const string PARAM_APPROVEDENTRY = "ApprovedEntry_";
    public const string PARAM_STARTDATE = "startDate_";
    public const string PARAM_ENDDATE = "endDate_";

    public const string FUNCTION_GETUSERBASE = "getuserbase";
    public const string FUNCTION_GETUSER = "getuser";
    public const string FUNCTION_REMOVEUSER = "removeuser";
    public const string FUNCTION_UPDATEUSER = "updateuser";
    public const string FUNCTION_ADDUSER = "adduser";
    public const string FUNCTION_USEREXISTS = "peekuser";
    public const string FUNCTION_VALIDUSER = "validateuser";
    public const string FUNCTION_LOGALARM = "addalarmlog";
    public const string FUNCTION_LOGACCESS = "addaccesslog";
    public const string FUNCTION_GETACCESSLOGS = "AccessReport";
    public const string FUNCTION_GETDOORACCESSLOGS = "doorreport";
    public const string FUNCTION_GETALARMLOG = "alarmreport";
    public const string FUNCTION_GETSUSPICIOUSUSERS = "suspicioususers";
    */
}
