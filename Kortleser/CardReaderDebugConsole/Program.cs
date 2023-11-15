using System.Net.Sockets;
using System.Text;
using CardReaderLibrary;

namespace CardReaderDebugConsole;

internal class Program
{
    static async Task Main(string[] args)
    {
        CardReader reader = new("127.0.0.1", 8000);

        reader.LogMessage += OnLogMessageReceived;

        try
        {
            await reader.SendRequestAsync("Hello, server");

            string response = await reader.ReceiveResponseAsync();
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
            reader.CloseConnection();
        }
    }

    private static void OnLogMessageReceived(object? sender, string message)
    {
        Console.WriteLine($"Log: {message}");
    }

    static void ConnectToListener()
    {
        using (TcpClient client = new TcpClient("127.0.0.1", 8000))
        {
            NetworkStream stream = client.GetStream();

            // Send request
            string request = "STATUS";
            byte[] requestBytes = Encoding.ASCII.GetBytes(request);
            stream.Write(requestBytes, 0, requestBytes.Length);

            // Recieve response
            byte[] responseBytes = new byte[4];
            int bytesRead = stream.Read(responseBytes, 0, responseBytes.Length);
            int response = BitConverter.ToInt32(responseBytes, 0);

            Console.WriteLine($"Recieved response: {response}");
        }
    }
}
