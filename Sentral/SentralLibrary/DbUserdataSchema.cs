using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
internal static class DbUserdataSchema
{
    // Table
    public const string TABLE_NAME = "userdata";

    // Columns
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
    public const string FUNCTION_GETACCESSLOGS = "accessreport";
    public const string FUNCTION_GETDOORACCESSLOGS = "doorreport";
    public const string FUNCTION_GETALARMLOG = "alarmreport";
    public const string FUNCTION_GETSUSPICIOUSUSERS = "suspicioususers";
}
