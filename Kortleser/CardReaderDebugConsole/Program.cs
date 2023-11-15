using System.Net.Sockets;
using System.Text;
using CardReaderLibrary;

namespace CardReaderDebugConsole;

internal class Program
{
    static async Task Main(string[] args)
    {
        TcpConnectionManager tcpConnection = new("127.0.0.1", 8000);

        tcpConnection.LogMessage += OnLogMessageReceived;

        try
        {
            await tcpConnection.SendRequestAsync("Hello, server");

            string response = await tcpConnection.ReceiveResponseAsync();
            Console.WriteLine($"Response: {response}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Press any key to close.");
            Console.ReadKey(true);
            tcpConnection.CloseConnection();
        }
    }

    private static void OnLogMessageReceived(object? sender, string message)
    {
        Console.WriteLine($"Log: {message}");
    }
}
