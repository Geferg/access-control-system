using SentralLibrary.DataClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Database.Processing;
public class DatabaseUserProcessing
{
    //TODO split up schema
    private readonly DatabaseAccess databaseAccess;

    public DatabaseUserProcessing(DatabaseAccess databaseAccess)
    {
        this.databaseAccess = databaseAccess;
    }

    public UserDetailedData? GetUser(string id)
    {
        UserDetailedData? result = new();

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_ID, id}
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.FUNCTION_GETUSER, parameters);
        DataRow row = dataTable.Rows[0];

        result.CardID = row[DbUserdataSchema.RETURN_ID].ToString();
        result.FirstName = row[DbUserdataSchema.RETURN_FIRSTNAME].ToString();
        result.LastName = row[DbUserdataSchema.RETURN_LASTNAME].ToString(); ;
        result.Email = row[DbUserdataSchema.RETURN_EMAIL].ToString();
        result.CardPin = row[DbUserdataSchema.RETURN_PIN].ToString();
        result.StartValidityTime = (DateTime)row[DbUserdataSchema.RETURN_STARTVALIDITY];
        result.EndValidityTime = (DateTime)row[DbUserdataSchema.RETURN_ENDVALIDITY];

        return result;
    }

    public List<UserSimpleData> GetUserbase()
    {
        List<UserSimpleData> result = new();
        DataTable table = databaseAccess.ExecuteQuery(DbUserdataSchema.FUNCTION_GETUSERBASE, new Dictionary<string, object>());

        foreach (DataRow user in table.Rows)
        {
            UserSimpleData newData = new()
            {
                CardID = user[DbUserdataSchema.RETURN_ID].ToString(),
                FirstName = user[DbUserdataSchema.RETURN_FIRSTNAME].ToString(),
                LastName = user[DbUserdataSchema.RETURN_LASTNAME].ToString()
            };

            result.Add(newData);
        }

        return result;
    }

    public bool UserExists(string id)
    {
        bool result = false;

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_ID, id}
        };


        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.FUNCTION_USEREXISTS, parameters);

        if (dataTable.Rows.Count > 0)
        {
            DataRow row = dataTable.Rows[0];
            result = Convert.ToBoolean(row[0]);
        }

        return result;
    }

    public bool RemoveUser(string id)
    {
        bool result = false;

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_ID, id}
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.FUNCTION_REMOVEUSER, parameters);

        if (dataTable.Rows.Count > 0)
        {
            DataRow row = dataTable.Rows[0];
            result = Convert.ToBoolean(row[0]);
        }

        return result;
    }

    public bool AddUser(UserDetailedData newUser)
    {
        bool result = false;

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_FIRSTNAME, newUser.FirstName ?? ""},
            {DbUserdataSchema.PARAM_LASTNAME, newUser.LastName ?? ""},
            {DbUserdataSchema.PARAM_EMAIL, newUser.Email ?? ""},
            {DbUserdataSchema.PARAM_ID, newUser.CardID ?? ""},
            {DbUserdataSchema.PARAM_PIN, newUser.CardPin ?? ""},
            {DbUserdataSchema.PARAM_STARTVALIDITY, newUser.StartValidityTime },
            {DbUserdataSchema.PARAM_ENDVALIDITY, newUser.EndValidityTime }
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.FUNCTION_ADDUSER, parameters);

        if (dataTable.Rows.Count > 0)
        {
            DataRow row = dataTable.Rows[0];
            result = Convert.ToBoolean(row[0]);
        }

        return result;
    }

    public bool EditUser(string previousId, UserDetailedData newUserData)
    {
        bool result = false;

        Dictionary<string, object> parameters = new()
        {
            {DbUserdataSchema.PARAM_PREVIOUSID, previousId},
            {DbUserdataSchema.PARAM_FIRSTNAME, newUserData.FirstName ?? ""},
            {DbUserdataSchema.PARAM_LASTNAME, newUserData.LastName ?? ""},
            {DbUserdataSchema.PARAM_EMAIL,  newUserData.Email ?? ""},
            {DbUserdataSchema.PARAM_ID, newUserData.CardID ?? "" },
            {DbUserdataSchema.PARAM_PIN, newUserData.CardPin ?? "" },
            {DbUserdataSchema.PARAM_STARTVALIDITY, newUserData.StartValidityTime },
            {DbUserdataSchema.PARAM_ENDVALIDITY,  newUserData.EndValidityTime }
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.FUNCTION_UPDATEUSER, parameters);

        if (dataTable.Rows.Count > 0)
        {
            DataRow row = dataTable.Rows[0];
            result = Convert.ToBoolean(row[0]);
        }

        return result;
    }
}