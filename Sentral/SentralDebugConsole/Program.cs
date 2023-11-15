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
        server.RequestReceived += OnRequestReceived;

        server.Start();

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey(true);

        server.Stop();
        server.LogMessage -= OnLogMessageReceived;
        server.RequestReceived -= OnRequestReceived;
    }

    private static void OnLogMessageReceived(object? sender, string message)
    {
        Console.WriteLine($"Log: {message}");
    }

    private static void OnRequestReceived(object? sender, RequestReceivedEventArgs request)
    {
        Console.WriteLine($"Request: {request.Message}");
        HandleRequest(request);
    }

    private static void HandleRequest(RequestReceivedEventArgs request)
    {
        //TODO implement different requests
    }

    static void HandleClient(object obj)
    {
        // Obsolete
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();

        // Gets request
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);

        switch (request)
        {
            case "STATUS":
                Console.WriteLine($"Recieved request {request} from {client.Client.RemoteEndPoint}");
                int connectedCount = GetConnectedCount();
                byte[] countResponse = BitConverter.GetBytes(connectedCount);
                stream.Write(countResponse, 0, countResponse.Length);
                break;

            case "LAST_IP":
                Console.WriteLine($"Recieved request {request} from {client.Client.RemoteEndPoint}");
                string lastIp = GetLastIp();
                byte[] ipResponse = Encoding.ASCII.GetBytes(lastIp);
                stream.Write(ipResponse, 0, ipResponse.Length);
                break;

            case "CLOSE":
                Console.WriteLine($"Recieved request {request} from {client.Client.RemoteEndPoint}");
                client.Close();
                return;

            default:
                Console.WriteLine("Unknown request");
                break;
        }
    }

    static string GetLastIp()
    {
        return "1";
    }

    static int GetConnectedCount()
    {
        return 1;
    }
}
