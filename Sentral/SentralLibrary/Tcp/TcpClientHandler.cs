using SentralLibrary.Services;
using SentralLibrary.Tcp.TcpRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Tcp;
public class TcpClientHandler : IClientManager
{
    public delegate void ClientDisconnectedHandler(TcpClientData clientData);
    public event ClientDisconnectedHandler? ClientDisconnected;

    private readonly TcpRequestProcessor requestProcessor;
    private readonly TcpResponseProcessor responseProcessor;
    private readonly TcpConnectionManager connectionManager;

    public TcpClientHandler(TcpConnectionManager connectionManager, DatabaseService databaseService)
    {
        this.connectionManager = connectionManager;
        requestProcessor = new(this, databaseService);
        responseProcessor = new();
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
                Response response = requestProcessor.ProcessClientRequest(clientData, request);
                responseProcessor.SendResponse(clientData, response);
            }
        }
        finally
        {
            clientData.TcpClient.Close();
        }
    }

    public bool IsDuplicateClientId(int clientId)
    {
        return connectionManager.IsDuplicateClientId(clientId);
    }
}