using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderLibrary;
public class TcpConnectionManager
{
    public string ServerAddress;

    private TcpClient client;
    private NetworkStream stream;
    public event EventHandler<string>? LogMessage;

    public TcpConnectionManager(string serverAddress, int serverPort)
    {
        ServerAddress = serverAddress;
        client = new TcpClient(serverAddress, serverPort);
        stream = client.GetStream();
    }

    public async Task<string> SendRequestAsync(string jsonRequest)
    {
        byte[] requestBytes = Encoding.UTF8.GetBytes(jsonRequest);
        await stream.WriteAsync(requestBytes);
        OnLogMessage($"Request sent: {jsonRequest}");

        byte[] responseBytes = new byte[1024];
        int bytesRead = await stream.ReadAsync(responseBytes);
        return Encoding.UTF8.GetString(responseBytes, 0, bytesRead);
    }

    // Deprecated
    public async Task<string> ReceiveResponseAsync()
    {
        byte[] responseBuffer = new byte[1024];
        int bytesRead = await stream.ReadAsync(responseBuffer);
        string response = Encoding.ASCII.GetString(responseBuffer, 0, bytesRead);
        return response;
    }

    public void CloseConnection()
    {
        stream.Close();
        client.Close();
        OnLogMessage("Connection closed.");
    }

    protected virtual void OnLogMessage(string message)
    {
        LogMessage?.Invoke(this, message);
    }
}
