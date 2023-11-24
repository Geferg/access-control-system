using SentralLibrary.Services;
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

    public TcpConnectionManager(int port, DatabaseService databaseService)
    {
        listener = new(IPAddress.Any, port);
        clients = new();
        cancellationTokenSource = new();
        clientHandler = new(this, databaseService);
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

    public int GetAuthorizedClientCount()
    {
        return clients.Where(c => c.IsAuthenticated).Count();
    }

    public int GetUnauthorizedClientCount()
    {
        return clients.Where(c => !c.IsAuthenticated).Count();
    }

    public List<int> GetClientIds()
    {
        List<int> result = new();
        foreach (var client in clients.Where(c => c.IsAuthenticated))
        {
            result.Add(client.ClientId);
        }

        return result;
    }

    private async Task ListenForClientsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
        {
            TcpClient newClient = await listener.AcceptTcpClientAsync(cancellationToken);
            TcpClientData client = new(newClient);

            // Handle each client in a separate task
            _ = Task.Run(async () =>
            {
                try
                {
                    clients.Add(client);
                    await clientHandler.HandleClientAsync(client, cancellationToken);
                }
                catch (ObjectDisposedException)
                {
                    // Handle specific exceptions if necessary
                }
                catch (SocketException)
                {
                    // Handle specific exceptions if necessary
                }
                finally
                {
                    clients.Remove(client);
                }
            });
        }
        catch (Exception ex)
        {
            // Log or handle the exception from AcceptTcpClientAsync
            // Consider if you need to break the loop or just log and continue
        }
        }
    }

    public bool IsDuplicateClientId(int clientId)
    {
        return clients.Any(client => client.ClientId == clientId);
    }
}
