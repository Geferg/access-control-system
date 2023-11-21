using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Database;
public class DatabaseAccess
{
    private readonly DatabaseConnection databaseConnection;

    public DatabaseAccess(DatabaseConnection databaseConnection)
    {
        this.databaseConnection = databaseConnection;
    }

    public DataTable ExecuteQuery(string query, Dictionary<string, object> parameters)
    {
        using var connection = databaseConnection.GetOpenConnection();
        using var command = new NpgsqlCommand(query, connection);
        foreach (var parameter in parameters)
        {
            command.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }

        using var adapter = new NpgsqlDataAdapter(command);
        var dataTable = new DataTable();
        adapter.Fill(dataTable);
        return dataTable;
    }
}