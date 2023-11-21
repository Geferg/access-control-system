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

    public TcpConnectionManager(int port)
    {
        listener = new(IPAddress.Any, port);
        clients = new();
    }

    public void Start()
    {
        listener.Start();
        ListenForClientsAsync();
    }

    public void Stop()
    {
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

    private async void ListenForClientsAsync()
    {
        // Does this go here?
        try
        {
            while (true)
            {
                TcpClient newClient = await listener.AcceptTcpClientAsync();
                TcpClientData client = new(newClient);
                lock (clients)
                {
                    clients.Add(client);
                    //TryLogMessage($"Client connected - {client.TcpClient.Client.RemoteEndPoint}");
                }

                _ = HandleClientAsync(client);
            }
        }
        catch (ObjectDisposedException)
        {
            //TryLogMessage("Lister has been stopped.");
        }
        catch (Exception ex)
        {
            //TryLogMessage($"Failed to listen: {ex}");
        }
    }
}
