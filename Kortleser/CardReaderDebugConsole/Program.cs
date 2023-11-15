using System.Net.Sockets;
using System.Text;

namespace CardReaderDebugConsole;

internal class Program
{
    static void Main(string[] args)
    {
        ConnectToListener();
        Console.ReadKey(true);
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
