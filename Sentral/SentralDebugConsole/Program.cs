using System.Net.Sockets;
using System.Net;
using System.Text;
using SentralLibrary;

namespace SentralDebugConsole;

internal class Program
{
    static void Main(string[] args)
    {
        var server = new TcpServer(8000);

        server.LogMessage += OnLogMessageReceived;
        server.RequestReceived += HandleRequest;

        server.Start();

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey(true);

        server.Stop();
        server.LogMessage -= OnLogMessageReceived;
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
