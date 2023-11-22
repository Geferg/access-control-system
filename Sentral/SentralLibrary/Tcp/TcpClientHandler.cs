using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Tcp;
public class TcpClientHandler
{
    public delegate void ClientDisconnectedHandler(TcpClientData clientData);
    public event ClientDisconnectedHandler? ClientDisconnected;

    private readonly TcpRequestProcessor requestProcessor;

    public TcpClientHandler(TcpRequestProcessor requestProcessor)
    {
        this.requestProcessor = requestProcessor;
    }

    public async Task HandleClientAsync(TcpClientData clientData, CancellationToken cancellationToken)
    {
        try
        {
            using NetworkStream stream = clientData.TcpClient.GetStream();
            byte[] buffer = new byte[1024];
            while (clientData.TcpClient.Connected && !cancellationToken.IsCancellationRequested)
            {
                int bytesRead = await stream.ReadAsync(buffer, cancellationToken);
                if (bytesRead == 0)
                {
                    // Client disconnected
                    break;
                }

                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                requestProcessor.ProccessClientRequest(clientData, request);
            }
        }
        finally
        {
            clientData.TcpClient.Close();
            ClientDisconnected?.Invoke(clientData);
        }
    }
}