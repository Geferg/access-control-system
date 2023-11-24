using SentralLibrary.Database.Config;
using SentralLibrary.Database.DataClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Database.Processing;
public class DatabaseUserProcessing
{
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
            {DatabaseSchema.Parameter_CardId, id}
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DatabaseSchema.Query_GetUser, parameters);

        if (dataTable.Rows.Count <= 0 )
        {
            return null;
        }

        DataRow row = dataTable.Rows[0];

        result.CardID = row[DatabaseSchema.Return_Id].ToString();
        result.FirstName = row[DatabaseSchema.Return_FirstName].ToString();
        result.LastName = row[DatabaseSchema.Return_LastName].ToString(); ;
        result.Email = row[DatabaseSchema.Return_Email].ToString();
        result.CardPin = row[DatabaseSchema.Return_Pin].ToString();
        result.StartValidityTime = (DateTime)row[DatabaseSchema.Return_StartValidity];
        result.EndValidityTime = (DateTime)row[DatabaseSchema.Return_EndValidity];

        return result;
    }

    public List<UserSimpleData> GetUserbase()
    {
        List<UserSimpleData> result = new();
        DataTable table = databaseAccess.ExecuteQuery(DatabaseSchema.Query_GetUserbase, new Dictionary<string, object>());

        foreach (DataRow user in table.Rows)
        {
            UserSimpleData newData = new()
            {
                CardID = user[DatabaseSchema.Return_Id].ToString(),
                FirstName = user[DatabaseSchema.Return_FirstName].ToString(),
                LastName = user[DatabaseSchema.Return_LastName].ToString()
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
            {DatabaseSchema.Parameter_CardId, id}
        };


        DataTable dataTable = databaseAccess.ExecuteQuery(DatabaseSchema.Query_PeekUser, parameters);

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
            {DatabaseSchema.Parameter_CardId, id}
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DatabaseSchema.Query_RemoveUser, parameters);

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
            {DatabaseSchema.Parameter_FirstName, newUser.FirstName ?? ""},
            {DatabaseSchema.Parameter_LastName, newUser.LastName ?? ""},
            {DatabaseSchema.Parameter_Email, newUser.Email ?? ""},
            {DatabaseSchema.Parameter_CardId, newUser.CardID ?? ""},
            {DatabaseSchema.Parameter_CardPin, newUser.CardPin ?? ""},
            {DatabaseSchema.Parameter_StartValidity, newUser.StartValidityTime },
            {DatabaseSchema.Parameter_EndValidity, newUser.EndValidityTime }
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DatabaseSchema.Query_AddUser, parameters);

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
            {DatabaseSchema.Parameter_OldCardId, previousId},
            {DatabaseSchema.Parameter_FirstName, newUserData.FirstName ?? ""},
            {DatabaseSchema.Parameter_LastName, newUserData.LastName ?? ""},
            {DatabaseSchema.Parameter_Email,  newUserData.Email ?? ""},
            {DatabaseSchema.Parameter_CardId, newUserData.CardID ?? "" },
            {DatabaseSchema.Parameter_CardPin, newUserData.CardPin ?? "" },
            {DatabaseSchema.Parameter_StartValidity, newUserData.StartValidityTime },
            {DatabaseSchema.Parameter_EndValidity,  newUserData.EndValidityTime }
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DatabaseSchema.Query_EditUser, parameters);

        if (dataTable.Rows.Count > 0)
        {
            DataRow row = dataTable.Rows[0];
            result = Convert.ToBoolean(row[0]);
        }

        return result;
    }

    public bool ValidateUser(string cardId, string cardPin)
    {
        bool result = false;

        Dictionary<string, object> parameters = new()
        {
            { DatabaseSchema.Parameter_CardId, cardId },
            { DatabaseSchema.Parameter_CardPin, cardPin }
        };

        DataTable dataTable = databaseAccess.ExecuteQuery(DatabaseSchema.Query_ValidateUser, parameters);

        if (dataTable.Rows.Count > 0)
        {
            DataRow row = dataTable.Rows[0];
            result = Convert.ToBoolean(row[0]);
        }

        return result;
    }

    public List<string> GetSuspiciousUserIds()
    {
        List<string> users = new();

        DataTable dataTable = databaseAccess.ExecuteQuery(DatabaseSchema.Query_GetSuspiciousUsers, new Dictionary<string, object>());

        foreach (DataRow row in dataTable.Rows)
        {
            string? userId = row[DatabaseSchema.Return_Id].ToString();
            if (userId != null)
            {
                users.Add(userId);
            }
        }

        return users;
    }
}