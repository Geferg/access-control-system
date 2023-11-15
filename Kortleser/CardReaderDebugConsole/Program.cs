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
    private static SerialConnectionManager serialConnection = new("COM6");
    private static TcpConnectionManager tcpConnection = new("127.0.0.1", 8000);

    static async Task Main(string[] args)
    {
        Console.WriteLine("\u001b]0;Access Point\u0007");

        tcpConnection.LogMessage += OnLogMessageReceived;
        serialConnection.LogMessage += OnLogMessageReceived;
        serialConnection.DataReceived += OnHardwareMessageReceived;

        try
        {
            serialConnection.OpenConnection();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Press any key to close.");
            Console.ReadKey(true);
            serialConnection.CloseConnection();
        }

        /* TCP COMM. TESTING
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
        }*/


    }

    private static async void OnHardwareMessageReceived(string message)
    {
        Console.WriteLine($"Received (hardware): {message}");
        await tcpConnection.SendRequestAsync(message);
        Console.WriteLine($"Sent (central): {message}");
        string response = await tcpConnection.ReceiveResponseAsync();
        Console.WriteLine($"Recieved (central): {response}");
    }

    private static void OnLogMessageReceived(object? sender, string message)
    {
        Console.WriteLine($"Log: {message}");
    }
}
