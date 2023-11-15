﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class TcpServer
{
    private TcpListener listener;
    private readonly List<TcpClient> clients;

    public event EventHandler<MessageReceivedEventArgs> RequestRecieved;

    public TcpServer(int port)
    {
        listener = new(IPAddress.Any, port);
        clients = new();
    }

    public void Start()
    {
        listener.Start();
        ListenForClientsAsync();
    }

    private async void ListenForClientsAsync()
    {
        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            lock (clients)
            {
                clients.Add(client);
            }

            _ = HandleClientAsync(client);
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                RequestRecieved?.Invoke(this, new MessageReceivedEventArgs(request));
            }
        }
        catch (Exception ex)
        {

        }
    }
}
