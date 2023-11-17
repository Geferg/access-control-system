using System.Net.Sockets;
using System.Net;
using System.Text;
using SentralLibrary;

namespace SentralDebugConsole;

internal class Program
{
    static UILogger logger = new();
    static DatabaseConnection connection = new("129.151.221.119", "5432", "599146", "Ha1FinDagIDag!", "599146");

    static void Main(string[] args)
    {
        connection.AttachLogger(logger);

        UserData? user = connection.GetUserData("1111");

        if (user != null)
        {
            Console.WriteLine($"{user.FirstName}");
        }

        Console.WriteLine("\u001b]0;Control Central\u0007");
        var server = new TcpServer(8000);

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

    private static void OnLogMessageReceived(object? sender, string message)
    {
        Console.WriteLine($"Log: {message}");
    }

    private static void HandleRequest(TcpClient client, string request, Action<TcpClient, string> respond)
    {
        string response = "response";

        respond(client, response);
    }
}
