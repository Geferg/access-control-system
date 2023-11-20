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

    // Functions (parameters always in alphabetical order)
    /// <summary>
    /// Gets the full name and card id of all users.
    /// </summary>
    public const string FUNCTION_GETUSERBASE = "getuserbase";
    /// <summary>
    /// Gets detailed information about one user based on COLUMN_ID.
    /// </summary>
    public const string FUNCTION_GETUSER = "getuser";
    /// <summary>
    /// Removes one user based on COLUMN_ID.
    /// </summary>
    public const string FUNCTION_REMOVEUSER = "removeuser";
    /// <summary>
    /// Updates one user based on COLUMN_ID.
    /// </summary>
    public const string FUNCTION_UPDATEUSER = "updateuser";
    /// <summary>
    /// Adds one new user if the given COLUMN_ID does not exist.
    /// </summary>
    public const string FUNCTION_ADDUSER = "adduser";
    /// <summary>
    /// Checks if user exists based on COLUMN_ID.
    /// </summary>
    public const string FUNCTION_USEREXISTS = "peekuser";
    /// <summary>
    /// Checks if a COLUMN_ID and COLUMN_PIN matches a user.
    /// </summary>
    public const string FUNCTION_VALIDUSER = "validateuser";

    public const string FUNCTION_LOGALARM = "";
    public const string FUNCTION_LOGACCESS = "";
}
