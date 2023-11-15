using System.Net.Sockets;
using System.Text;
using CardReaderLibrary;

/*
 * Concerns:
 * - Handling partial messages
 * - Resource management for tcpclient and networkstream (using or dispose)
 * - Fixed buffer size
 * - Testing different network scenarios
 */

namespace CardReaderDebugConsole;
internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("\u001b]0;Access Point\u0007");
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
