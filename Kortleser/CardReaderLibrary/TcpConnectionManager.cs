using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderLibrary;
public class TcpConnectionManager
{
    private TcpClient client;
    private NetworkStream stream;
    public event EventHandler<string>? LogMessage;

    public TcpConnectionManager(string serverAddress, int serverPort)
    {
        client = new TcpClient(serverAddress, serverPort);
        stream = client.GetStream();
    }

    public async Task SendRequestAsync(string request)
    {
        byte[] requestBytes = Encoding.ASCII.GetBytes(request);
        await stream.WriteAsync(requestBytes, 0, requestBytes.Length);
        OnLogMessage("Request sent.");
    }

    public async Task<string> ReceiveResponseAsync()
    {
        byte[] responseBuffer = new byte[1024];
        int bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
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
