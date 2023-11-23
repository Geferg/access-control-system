﻿using SentralLibrary.Services;
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

    private async Task ListenForClientsAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                TcpClient newClient = await listener.AcceptTcpClientAsync(cancellationToken);
                TcpClientData client = new(newClient);
                clients.Add(client);
                await clientHandler.HandleClientAsync(client, cancellationToken);
                clients.Remove(client);
            }
        }
        catch (ObjectDisposedException)
        {
            //TODO consider handling listener stopped
        }
    }

    public bool IsDuplicateClientId(int clientId)
    {
        return clients.Any(client => client.ClientId == clientId);
    }
}