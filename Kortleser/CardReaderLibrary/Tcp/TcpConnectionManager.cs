using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderLibrary.Tcp;
public class TcpConnectionManager
{
    public string ServerAddress;
    public int ServerPort;
    public bool Connected => client.Connected;

    private readonly TcpClient client;
    private NetworkStream? stream;
    public event EventHandler<string>? LogMessage;

    public TcpConnectionManager(string serverAddress, int serverPort)
    {
        ServerAddress = serverAddress;
        ServerPort = serverPort;
        client = new TcpClient();
    }

    public void OpenConnection()
    {
        client.Connect(ServerAddress, ServerPort);
        stream = client.GetStream();
    }

    public async Task<string> SendRequestAsync(string jsonRequest)
    {
        if (stream == null)
        {
            throw new Exception();
        }

        byte[] requestBytes = Encoding.UTF8.GetBytes(jsonRequest);
        await stream.WriteAsync(requestBytes);

        byte[] responseBytes = new byte[1024];
        int bytesRead = await stream.ReadAsync(responseBytes);
        return Encoding.UTF8.GetString(responseBytes, 0, bytesRead);
    }

    public void CloseConnection()
    {
        stream?.Close();
        client.Close();
    }
}
