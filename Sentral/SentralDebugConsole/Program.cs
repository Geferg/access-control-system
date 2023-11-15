using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SentralDebugConsole;

internal class Program
{
    private static int clientCount = 0;
    static void Main(string[] args)
    {
        StartTcpServer();
        Console.ReadKey(true);
    }

    static void StartTcpServer()
    {
        var listener = new TcpListener(IPAddress.Any, 8000);
        listener.Start();

        Console.WriteLine("Listener started, waiting for connections...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");
            ThreadPool.QueueUserWorkItem(HandleClient, client);
        }
    }

    static void HandleClient(object obj)
    {
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

    static int TestConnectionMethod(string request)
    {
        Console.WriteLine("Performing operation...");

        return 1;
    }
}
