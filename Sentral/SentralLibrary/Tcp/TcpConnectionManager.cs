using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Tcp;
public class TcpConnectionManager
{
    private readonly TcpListener listener;
    private readonly List<TcpClientData> clients;
    private readonly TcpClientHandler clientHandler;
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly TcpRequestProcessor processor;

    public TcpConnectionManager(int port)
    {
        listener = new(IPAddress.Any, port);
        clients = new();
        cancellationTokenSource = new();

        processor = new();
        clientHandler = new(processor);
        clientHandler.ClientDisconnected += ClientHandler_ClientDisconnected;
    }

    public void Start()
    {
        listener.Start();
        _ = ListenForClientsAsync(cancellationTokenSource.Token);
    }

    public void Stop()
    {
        cancellationTokenSource.Cancel();
        listener.Stop();
        lock (clients)
        {
            foreach (var client in clients)
            {
                client.TcpClient.Close();
            }
            clients.Clear();
        }
    }

    private async Task ListenForClientsAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                TcpClient newClient = await listener.AcceptTcpClientAsync(cancellationToken);
                TcpClientData client = new(newClient);
                lock (clients)
                {
                    clients.Add(client);
                }

                _ = clientHandler.HandleClientAsync(client, cancellationToken);
            }
        }
        catch (ObjectDisposedException)
        {
            //TODO consider handling listener stopped
        }
    }

    private void ClientHandler_ClientDisconnected(TcpClientData clientData)
    {
        lock (clients)
        {
            clients.Remove(clientData);
        }
    }
}
