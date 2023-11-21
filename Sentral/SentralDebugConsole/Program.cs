using System.Net.Sockets;
using System.Net;
using System.Text;
using SentralLibrary;
using SentralLibrary.DataClasses;

namespace SentralDebugConsole;

internal class Program
{
    static UILogger logger = new();
    static DatabaseConnectionOld connection = new("129.151.221.119", "5432", "599146", "Ha1FinDagIDag!", "599146");

    static void Main(string[] args)
    {
        connection.AttachLogger(logger);

        UserDetailedData? user = connection.GetUser("1111");

        if (user != null)
        {
            Console.WriteLine($"{user.FirstName}");
        }

        Console.WriteLine("\u001b]0;Control Central\u0007");
        var server = new TcpConnectionOld(8000);

        server.AttachLogger(logger);

        logger.LogMessageEvent += OnLogMessageReceived;
        server.RequestReceived += HandleRequest;

        server.Start();

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey(true);

        server.Stop();
        logger.LogMessageEvent -= OnLogMessageReceived;
        server.RequestReceived -= HandleRequest;
    }

    // Deprecated
    private static void OnLogMessageReceived(object? sender, string message)
    {
        Console.WriteLine($"Log: {message}");
    }

    // Deprecated
    private static void HandleRequest(TcpClient client, string request, Action<TcpClient, string> respond)
    {
        string response = "response";

        respond(client, response);
    }
}
