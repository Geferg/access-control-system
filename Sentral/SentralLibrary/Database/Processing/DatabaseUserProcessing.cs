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
            {DbUserdataSchema.Parameter_CardId, id}
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.Query_GetUser, parameters);
        DataRow row = dataTable.Rows[0];

        result.CardID = row[DbUserdataSchema.Return_Id].ToString();
        result.FirstName = row[DbUserdataSchema.Return_FirstName].ToString();
        result.LastName = row[DbUserdataSchema.Return_LastName].ToString(); ;
        result.Email = row[DbUserdataSchema.Return_Email].ToString();
        result.CardPin = row[DbUserdataSchema.Return_Pin].ToString();
        result.StartValidityTime = (DateTime)row[DbUserdataSchema.Return_StartValidity];
        result.EndValidityTime = (DateTime)row[DbUserdataSchema.Return_EndValidity];

        return result;
    }

    public List<UserSimpleData> GetUserbase()
    {
        List<UserSimpleData> result = new();
        DataTable table = databaseAccess.ExecuteQuery(DbUserdataSchema.Query_GetUserbase, new Dictionary<string, object>());

        foreach (DataRow user in table.Rows)
        {
            UserSimpleData newData = new()
            {
                CardID = user[DbUserdataSchema.Return_Id].ToString(),
                FirstName = user[DbUserdataSchema.Return_FirstName].ToString(),
                LastName = user[DbUserdataSchema.Return_LastName].ToString()
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
            {DbUserdataSchema.Parameter_CardId, id}
        };


        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.Query_PeekUser, parameters);

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
            {DbUserdataSchema.Parameter_CardId, id}
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.Query_RemoveUser, parameters);

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
            {DbUserdataSchema.Parameter_FirstName, newUser.FirstName ?? ""},
            {DbUserdataSchema.Parameter_LastName, newUser.LastName ?? ""},
            {DbUserdataSchema.Parameter_Email, newUser.Email ?? ""},
            {DbUserdataSchema.Parameter_CardId, newUser.CardID ?? ""},
            {DbUserdataSchema.Parameter_CardPin, newUser.CardPin ?? ""},
            {DbUserdataSchema.Parameter_StartValidity, newUser.StartValidityTime },
            {DbUserdataSchema.Parameter_EndValidity, newUser.EndValidityTime }
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.Query_AddUser, parameters);

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
            {DbUserdataSchema.Parameter_OldCardId, previousId},
            {DbUserdataSchema.Parameter_FirstName, newUserData.FirstName ?? ""},
            {DbUserdataSchema.Parameter_LastName, newUserData.LastName ?? ""},
            {DbUserdataSchema.Parameter_Email,  newUserData.Email ?? ""},
            {DbUserdataSchema.Parameter_CardId, newUserData.CardID ?? "" },
            {DbUserdataSchema.Parameter_CardPin, newUserData.CardPin ?? "" },
            {DbUserdataSchema.Parameter_StartValidity, newUserData.StartValidityTime },
            {DbUserdataSchema.Parameter_EndValidity,  newUserData.EndValidityTime }
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DbUserdataSchema.Query_EditUser, parameters);

        if (dataTable.Rows.Count > 0)
        {
            DataRow row = dataTable.Rows[0];
            result = Convert.ToBoolean(row[0]);
        }

        return result;
    }
}