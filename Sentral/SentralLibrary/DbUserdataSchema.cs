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
    public const string COLUMN_FIRSTNAME = "firstname";
    public const string COLUMN_LASTNAME = "lastname";
    public const string COLUMN_EMAIL = "email";
    public const string COLUMN_ID = "cardid";
    public const string COLUMN_PIN = "cardpin";
    public const string COLUMN_STARTVALIDITY = "validstart";
    public const string COLUMN_ENDVALIDITY = "validend";

    public const string PARAM_FIRSTNAME = "firstname";
    public const string PARAM_LASTNAME = "lastname";
    public const string PARAM_EMAIL = "email";
    public const string PARAM_ID = "cardid";
    public const string PARAM_PIN = "cardpin";
    public const string PARAM_STARTVALIDITY = "validstart";
    public const string PARAM_ENDVALIDITY = "validend";

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

    // Parameters?

}
